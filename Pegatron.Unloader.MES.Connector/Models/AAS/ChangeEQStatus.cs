using Newtonsoft.Json;

namespace Pegatron.Unloader.MES.Connector
{
    public class ChangeEQStatus : AasBaseResponse
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
        /// 狀態代號。範例：1,2,3…
        /// </summary>
        [JsonProperty("Status")]
        public string StatusReq { get; set; }
        /// <summary>
        /// 片子的編號。
        /// </summary>
        [JsonProperty("PanelID")]
        public string PanelID { get; set; }
    }
}
