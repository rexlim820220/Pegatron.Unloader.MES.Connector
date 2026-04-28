using Newtonsoft.Json;
using System.Collections.Generic;

namespace Pegatron.Unloader.MES.Connector
{
    public class CheckLoader : AasBaseResponse
    {
        /// <summary>
        /// 線別名稱。範例：ITCM-53001
        /// </summary>
        [JsonProperty("SerialNo")]
        public string SerialNoReq { get; set; }
        /// <summary>
        /// 任一子設備。範例：ITCM-53001-1
        /// </summary>
        [JsonProperty("EQName")]
        public string EQName { get; set; }
        /// <summary>
        /// 工單號碼。範例：2127829.00
        /// </summary>
        [JsonProperty("Lot")]
        public string Lot { get; set; }
        /// <summary>
        /// 檢查項目(*此Task檢查項目 For IT壓模機)
        /// </summary>
        [JsonProperty("Task")]
        public List<string> Task { get; set; } = new List<string>();
    }
}