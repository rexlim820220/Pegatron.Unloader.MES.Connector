using Newtonsoft.Json;

namespace Pegatron.Unloader.MES.Connector
{
    public class CheckTime : AasBaseResponse
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
    }
}