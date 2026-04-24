using Newtonsoft.Json;
using System.Collections.Generic;

namespace Models.AAS
{
    public class CheckLoaderRequest
    {
        [JsonProperty("SerialNo")]
        public string SerialNo { get; set; }

        [JsonProperty("EQName")]
        public string EqName { get; set; }

        [JsonProperty("Lot")]
        public string Lot { get; set; }

        /// <summary>
        /// 檢查項目 (例如: ["Lotcheck"])
        /// </summary>
        [JsonProperty("Task")]
        public List<string> Task { get; set; } = new List<string>();
    }
}
