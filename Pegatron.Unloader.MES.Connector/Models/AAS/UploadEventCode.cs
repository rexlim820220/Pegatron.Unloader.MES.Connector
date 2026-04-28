using Newtonsoft.Json;

namespace Pegatron.Unloader.MES.Connector
{
    public class UploadEventCode : AasBaseResponse
    {
        [JsonProperty("SerialNo")]
        public string SerialNoReq { get; set; }
        [JsonProperty("EQName")]
        public string EQName { get; set; }
        [JsonProperty("EventCode")]
        public string EventCode { get; set; }
        [JsonProperty("EventCodeDesc")]
        public string EventCodeDesc { get; set; }
        [JsonProperty("OccurDate")]
        public string OccurDate { get; set; }
        [JsonProperty("EventStatus")]
        public string EventStatus { get; set; }
        [JsonProperty("SubeqStatus")]
        public string SubeqStatus { get; set; }
        [JsonProperty("PanelID")]
        public string PanelID { get; set; }
    }
}
