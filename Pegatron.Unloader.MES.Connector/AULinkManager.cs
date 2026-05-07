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
            var config = ConfigHelper.GetConfig();
            _baseUrl = config.BaseUrl.TrimEnd('/');
            _apiKey = config.ApiKey;
        }

        private async Task<string> InternalPostAsync(string path, object payload, bool isMvix = false)
        {
            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

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
                _ = Task.Run(() => LogHelper.WriteLog($"MVIX", resJson, resJson, sw.ElapsedMilliseconds));

                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();
                resJson = $"[Exception] {ex.Message}";
                _ = Task.Run(() => LogHelper.WriteLog($"MVIX", resJson, resJson, sw.ElapsedMilliseconds));
                return new MvixResponse { Status = "error", Message = ex.Message };
            }
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
            => await ExecuteAasAsync<CheckLoader, CheckLoader>("CheckLoader", data);

        public async Task<CheckPanel> CheckPanelAsync(CheckPanel data)
            => await ExecuteAasAsync<CheckPanel, CheckPanel>("CheckPanel", data);

        public async Task<UploadData> UploadDataAsync(UploadData data)
            => await ExecuteAasAsync<UploadData, UploadData>("UploadData", data);

        public async Task<UploadEventCode> UploadEventCodeAsync(UploadEventCode data)
            => await ExecuteAasAsync<UploadEventCode, UploadEventCode>("UploadEventCode", data);

        public async Task<CheckTime> CheckTimeAsync(CheckTime data)
            => await ExecuteAasAsync<CheckTime, CheckTime>("CheckTime", data);

        public async Task<ChangeEQMode> ChangeEQModeAsync(ChangeEQMode data)
            => await ExecuteAasAsync<ChangeEQMode, ChangeEQMode>("ChangeEQMode", data);

        // --- MVIX 的入口 ---
        public async Task<MvixResponse> UploadMvixAsync(MvixRequest data)
        {
            return await ExecuteMvixAsync(data);
        }
    }
}
