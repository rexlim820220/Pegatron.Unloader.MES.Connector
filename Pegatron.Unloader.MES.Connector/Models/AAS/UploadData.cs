using Newtonsoft.Json;
using System.Collections.Generic;

namespace Pegatron.Unloader.MES.Connector
{
    public class UploadData : AasBaseResponse
    {
        /// <summary>
        /// 線別名稱。範例：ITCM-52002
        /// </summary>
        [JsonProperty("SerialNo")]
        public string SerialNoReq { get; set; }
        /// <summary>
        /// 任一子設備。範例：ITCM-52002-1
        /// </summary>
        [JsonProperty("EQName")]
        public string EQName { get; set; }
        /// <summary>
        /// 上傳PanelID的資料。
        /// </summary>
        [JsonProperty("Data")]
        public List<DataItem> Data { get; set; } = new List<DataItem>();
    }

    public class DataItem
    {
        [JsonProperty("Key")]
        public string Key { get; set; }
        [JsonProperty("Value")]
        public string Value { get; set; }
    }
}
