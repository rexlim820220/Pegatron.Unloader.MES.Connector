using System;
using Models.AAS;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pegatron.Unloader.MES.Connector
{
    public class AULinkManager
    {
        private readonly string _baseUrl;
        private static readonly HttpClient _client;
        private const string ApiKey = "433576dd-1489-44e5-b19c-baa9c9463de";

        static AULinkManager()
        {
            System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)3072;
            _client = new HttpClient();
        }

        public AULinkManager(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
        }

        private async Task<TRes> ExecuteAasAsync<TReq, TRes>(string serviceName, TReq data) where TRes : AasBaseResponse, new()
        {
            try
            {
                var payload = new { service_name = serviceName, request_data = data };
                string json = await InternalPostAsync("/api/v1/aas/dispatch", payload, false);
                return JsonConvert.DeserializeObject<TRes>(json);
            }
            catch (Exception ex)
            {
                return AasBaseResponse.CreateError<TRes>(serviceName, ex.Message);
            }
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

        // --- 公開方法 (Caller 呼叫區) ---

        public async Task<CheckLoaderResponse> CheckLoaderAsync(CheckLoaderRequest data)
        {
            try
            {
                var payload = new
                {
                    service_name = "CheckLoader",
                    request_data = data
                };
                string json = await InternalPostAsync("/api/v1/aas/dispatch", payload);
                return JsonConvert.DeserializeObject<CheckLoaderResponse>(json);
            }
            catch (Exception ex)
            {
                return AasBaseResponse.CreateError<CheckLoaderResponse>("CheckLoader", ex.Message);
            }
        }

        public async Task<UploadData> UploadDataAsync(UploadData data)
             => await ExecuteAasAsync<UploadData, UploadData>("UploadData", data);
    }
}
