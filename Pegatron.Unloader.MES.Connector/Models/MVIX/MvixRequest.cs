using Newtonsoft.Json;
using System.Collections.Generic;

namespace Models.MVIX
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
        [JsonProperty("p_id")]
        public string PId { get; set; }

        [JsonProperty("raw_value")]
        public string RawValue { get; set; }

        /// <summary>
        /// 格式：yyyyMMddHHmmss
        /// </summary>
        [JsonProperty("run_date_time")]
        public string RunDateTime { get; set; }
    }
}
