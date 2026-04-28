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
        private static readonly HttpClient _client;
        private const string ApiKey = "433576dd-1489-44e5-b19c-baa9c9463de";

        static AULinkManager()
        {
            System.Net.ServicePointManager.SecurityProtocol =
            System.Net.SecurityProtocolType.Tls12 | (System.Net.SecurityProtocolType)12288;
            System.Net.WebRequest.DefaultWebProxy.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
            _client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(8)
            };
        }

        public AULinkManager(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
        }

        private async Task<string> InternalPostAsync(string path, object payload, bool isMvix = false)
        {
            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _client.DefaultRequestHeaders.Remove("apiKey");
            if (isMvix)
            {
                _client.DefaultRequestHeaders.Add("apiKey", ApiKey);
            }

            var response = await _client.PostAsync($"{_baseUrl}{path}", content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private async Task<TRes> ExecuteAasAsync<TReq, TRes>(string serviceName, TReq data) where TRes : AasBaseResponse, new()
        {
            try
            {
                var payload = new { service_name = serviceName, request_data = data };
                string json = await InternalPostAsync("/api/v1/aas/dispatch", payload, false);
                var result = JsonConvert.DeserializeObject<TRes>(json);

                if (result.ServiceName != serviceName && !string.IsNullOrEmpty(result.ServiceName))
                {
                    LogHelper.WriteWarning($"Service Name Mismatch: Req={serviceName}, Res={result.ServiceName}");
                }
                result.ServiceName = serviceName;
                return result;
            }
            catch (Exception ex)
            {
                return AasBaseResponse.CreateError<TRes>(serviceName, ex.Message);
            }
        }

        private async Task<MvixResponse> ExecuteMvixAsync(MvixRequest data)
        {
            try
            {
                string json = await InternalPostAsync("/api/v1/mvix/dispatch", data, true);
                return JsonConvert.DeserializeObject<MvixResponse>(json);
            }
            catch (Exception ex)
            {
                return new MvixResponse { Status = "error", Message = ex.Message };
            }
        }

        // --- AAS 的入口 ---
        public async Task<ChangeEQStatus> ChangeEQStatusAsync(ChangeEQStatus data)
            => await ExecuteAasAsync<ChangeEQStatus, ChangeEQStatus>("ChangeEQStatus", data);

        public async Task<UploadRecipe> UploadRecipeAsync(string clientIp, UploadRecipe data)
        {
            try
            {
                var payload = new
                {
                    service_name = "UploadRecipe",
                    client_ip = clientIp,
                    request_data = data
                };

                string json = await InternalPostAsync("/api/v1/aas/dispatch", payload, false);
                return JsonConvert.DeserializeObject<UploadRecipe>(json);
            }
            catch (Exception ex)
            {
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
