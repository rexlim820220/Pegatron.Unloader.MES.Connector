using Newtonsoft.Json;

namespace Pegatron.Unloader.MES.Connector
{
    public class CheckPanel : AasBaseResponse
    {
        /// <summary>
        /// 工單號碼。
        /// </summary>
        [JsonProperty("Lot")]
        public string Lot { get; set; }
        /// <summary>
        /// 片子的編號。
        /// </summary>
        [JsonProperty("PanelID")]
        public string PanelID { get; set; }
    }
}
