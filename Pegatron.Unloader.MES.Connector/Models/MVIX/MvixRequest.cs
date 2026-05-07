using Newtonsoft.Json;
using System.Collections.Generic;
using Pegatron.Unloader.MES.Connector.Helpers;

namespace Pegatron.Unloader.MES.Connector
{
    public class MvixRequest
    {
        [JsonProperty("eq_id")]
        public string EqId { get; set; }

        [JsonProperty("raw_datas")]
        public List<MvixRawData> RawDatas { get; set; } = new List<MvixRawData>();
    }

    public class MvixRawData
    {
        /// <summary>
        /// 對應 CSV 的 alias_name (例如: Mode_Status, EQ_Status)
        /// </summary>
        [JsonProperty("p_id")]
        public string PId { get; set; }

        /// <summary>
        /// 對應 CSV 的 config_type。
        /// 雖然這裡用 string，但傳入值必須符合 CSV 規範的 Double 或 String。
        /// </summary>
        [JsonProperty("raw_value")]
        public string RawValue { get; set; }

        /// <summary>
        /// 格式：yyyyMMddHHmmss (使用 DateTimeHelper.ToMvixFormat)
        /// </summary>
        [JsonProperty("run_date_time")]
        public string RunDateTime { get; set; }
    }
}
