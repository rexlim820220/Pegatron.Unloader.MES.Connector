using Newtonsoft.Json;

namespace Models.AAS
{
    public class AasBaseResponse
    {
        public static T CreateError<T>(string serviceName, string errorMessage) where T : AasBaseResponse, new()
        {
            return new T
            {
                ServiceName = serviceName,
                Status = "error",
                Result = "1",
                Message = $"[DLL_INTERNAL_ERROR] {errorMessage}"
            };
        }

        [JsonProperty("service_name")]
        public string ServiceName { get; set; }

        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }

        /// <summary>
        /// 服務處理結果: 0 (success), 1 (failure), 2 (warning)
        /// </summary>
        [JsonProperty("result", NullValueHandling = NullValueHandling.Ignore)]
        public object Result { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("SerialNo")]
        public string SerialNo { get; set; }

        [JsonProperty("EQName")]
        public string EqName { get; set; }
    }
}
