using Newtonsoft.Json;

namespace Pegatron.Unloader.MES.Connector
{
    public class AasBaseResponse
    {
        public static T CreateError<T>(string serviceName, string errorMessage) where T : AasBaseResponse, new()
        {
            return new T
            {
                ServiceName = serviceName,
                Status = "error",
                Result = "0",
                Message = $"[DLL_INTERNAL_ERROR] {errorMessage}"
            };
        }

        [JsonProperty("service_name", NullValueHandling = NullValueHandling.Ignore)]
        public string ServiceName { get; set; }

        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }

        /// <summary>
        /// 服務處理結果: 1 (成功), 2 (業務失敗/警告), 0 (系統例外/資料缺失)
        /// </summary>
        [JsonProperty("result", NullValueHandling = NullValueHandling.Ignore)]
        public object Result { get; set; }

        [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
        public string Code { get; set; }

        [JsonProperty("unit", NullValueHandling = NullValueHandling.Ignore)]
        public string Unit { get; set; }

        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }
        [JsonIgnore]
        public bool IsSuccess => Status == "success" && (Result?.ToString() == "1");
        [JsonIgnore]
        public string ResultType
        {
            get
            {
                if (IsSuccess) return "Success";
                if (Result?.ToString() == "2") return "Warning/BusinessFail";
                return "SystemError/Exception";
            }
        }
    }
}
