using System;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Pegatron.Unloader.MES.Connector.Helpers;

namespace Pegatron.Unloader.MES.Connector
{
    public class AULinkManager
    {
        private readonly string _baseUrl;
        private readonly string _apiKey;
        private static readonly HttpClient _client;
        private readonly ConfigModel _config;

        static AULinkManager()
        {
            var config = ConfigHelper.GetConfig();
            System.Net.ServicePointManager.SecurityProtocol =
            System.Net.SecurityProtocolType.Tls12 | (System.Net.SecurityProtocolType)12288;
            System.Net.WebRequest.DefaultWebProxy.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
            _client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds)
            };
        }

        public AULinkManager()
        {
            _config = ConfigHelper.GetConfig();
            _baseUrl = _config.BaseUrl.TrimEnd('/');
            _apiKey = _config.ApiKey;
        }

        private async Task<string> InternalPostAsync(string path, object payload, bool isMvix = false)
        {
            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            if (_config.IsMockMode)
            {
                await Task.Delay(100);
                return "{\"result\": \"OK\", \"service_name\": \"MockMode\"}";
            }

            _client.DefaultRequestHeaders.Remove("apiKey");
            if (isMvix)
            {
                _client.DefaultRequestHeaders.Add("apiKey", _apiKey );
            }

            var response = await _client.PostAsync($"{_baseUrl}{path}", content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private async Task<TRes> ExecuteAasAsync<TReq, TRes>(string serviceName, TReq data) where TRes : AasBaseResponse, new()
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            string json;

            try
            {
                var payload = new { service_name = serviceName, request_data = data };
                json = await InternalPostAsync("/api/v1/aas/dispatch", payload, false);
                var result = JsonConvert.DeserializeObject<TRes>(json);

                sw.Stop();
                if (result.ServiceName != serviceName && !string.IsNullOrEmpty(result.ServiceName))
                {
                    LogHelper.WriteWarning($"Service Name Mismatch: Req={serviceName}, Res={result.ServiceName}");
                }
                LogHelper.WriteLog(serviceName, json, json, sw.ElapsedMilliseconds);

                result.ServiceName = serviceName;
                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();
                json = $"Exception: {ex.Message}";
                LogHelper.WriteLog(serviceName, json, json, sw.ElapsedMilliseconds);
                return AasBaseResponse.CreateError<TRes>(serviceName, ex.Message);
            }
        }

        private async Task<MvixResponse> ExecuteMvixAsync(MvixRequest data)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            string resJson = JsonConvert.SerializeObject(data);
            try
            {
                resJson = await InternalPostAsync("/api/v1/mvix/dispatch", data, true);
                var result = JsonConvert.DeserializeObject<MvixResponse>(resJson);

                sw.Stop();
                Task.Run(() => LogHelper.WriteLog("MVIX", resJson, resJson, sw.ElapsedMilliseconds));

                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();
                resJson = $"[Exception] {ex.Message}";
                Task.Run(() => LogHelper.WriteLog("MVIX", resJson, resJson, sw.ElapsedMilliseconds));
                return new MvixResponse { Status = "error", Message = ex.Message };
            }
        }

        public string GetFallbackPanelId(string lotId, int sequence)
        {
            return $"{lotId}E{sequence:D2}";
        }

        // --- AAS 的入口 ---
        public async Task<ChangeEQStatus> ChangeEQStatusAsync(ChangeEQStatus data)
            => await ExecuteAasAsync<ChangeEQStatus, ChangeEQStatus>("ChangeEQStatus", data);

        public async Task<UploadRecipe> UploadRecipeAsync(string clientIp, UploadRecipe data)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            string json;
            try
            {
                var payload = new
                {
                    service_name = "UploadRecipe",
                    client_ip = clientIp,
                    request_data = data
                };

                json = await InternalPostAsync("/api/v1/aas/dispatch", payload, false);
                var result = JsonConvert.DeserializeObject<UploadRecipe>(json);
                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();
                json = $"[Exception] {ex.Message}";
                LogHelper.WriteLog("UploadRecipe", json, json, sw.ElapsedMilliseconds);
                return AasBaseResponse.CreateError<UploadRecipe>("UploadRecipe", ex.Message);
            }
        }

        public async Task<CheckLoader> CheckLoaderAsync(CheckLoader data)
        {
            if (!_config.ServiceSwitches.EnableLotCheck)
            {
                LogHelper.WriteWarning("CheckLoader (LotCheck) is disabled by config.");
                return new CheckLoader { Result = "SKIP" };
            }
            return await ExecuteAasAsync<CheckLoader, CheckLoader>("CheckLoader", data);
        }

        public async Task<CheckPanel> CheckPanelAsync(CheckPanel data, int sequence = 1)
        {
            if (string.IsNullOrWhiteSpace(data.PanelID) || data.PanelID.ToUpper() == "FAIL")
            {
                data.PanelID = GetFallbackPanelId(data.Lot, sequence);
                LogHelper.WriteWarning($"PanelID Read Fail, auto-generated: {data.PanelID}");
            }

            return await ExecuteAasAsync<CheckPanel, CheckPanel>("CheckPanel", data);
        }

        public async Task<UploadData> UploadDataAsync(UploadData data)
        {
            try
            {
                data.EQName = data.EQName ?? _config.EqId;

                if (_config.MappingIDs != null && data.Data != null)
                {
                    foreach (var item in data.Data)
                    {
                        if (string.IsNullOrEmpty(item.Key) && _config.MappingIDs.ContainsKey("DefaultTempID"))
                        {
                            item.Key = _config.MappingIDs["DefaultTempID"];
                        }
                    }
                }
                return await ExecuteAasAsync<UploadData, UploadData>("UploadData", data);
            }
            catch (TaskCanceledException)
            {
                LogHelper.WriteWarning("UploadData Timeout! Consider increasing TimeoutSeconds in config.");
                return AasBaseResponse.CreateError<UploadData>("UploadData", "TIMEOUT");
            }
            catch (Exception ex)
            {
                return AasBaseResponse.CreateError<UploadData>("UploadData", $"Internal: {ex.Message}");
            }
        }


        public async Task<UploadEventCode> UploadEventCodeAsync(UploadEventCode data)
            => await ExecuteAasAsync<UploadEventCode, UploadEventCode>("UploadEventCode", data);

        public async Task<CheckTime> CheckTimeAsync(CheckTime data)
        {
            var result = await ExecuteAasAsync<CheckTime, CheckTime>("CheckTime", data);

            if (result != null && !string.IsNullOrEmpty(result.SystemTime))
            {
                DateTime serverTime;
                if (DateTime.TryParse(result.SystemTime, out serverTime))
                {
                    bool isSuccess = DateTimeHelper.SyncSystemTime(serverTime);
                    LogHelper.WriteLog("SystemTimeSync", $"Sync Status: {isSuccess}", result.SystemTime, 0);
                }
                else
                {
                    LogHelper.WriteWarning($"Time Format Error: {result.SystemTime}");
                }
            }
            return result;
        }

        public async Task<ChangeEQMode> ChangeEQModeAsync(ChangeEQMode data)
            => await ExecuteAasAsync<ChangeEQMode, ChangeEQMode>("ChangeEQMode", data);

        // --- MVIX 的入口 ---
        public async Task<MvixResponse> UploadMvixAsync(MvixRequest data)
        {
            return await ExecuteMvixAsync(data);
        }
    }
}
