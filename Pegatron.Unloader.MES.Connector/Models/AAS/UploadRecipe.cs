using Newtonsoft.Json;
using System.Collections.Generic;

namespace Pegatron.Unloader.MES.Connector
{
    public class UploadRecipe : AasBaseResponse
    {
        public string ClientIp { get; set; }
        [JsonProperty("SerialNo")]
        public string SerialNoReq { get; set; }
        [JsonProperty("EQName")]
        public string EQName { get; set; }
        [JsonProperty("Recipe")]
        public List<RecipeItem> Recipe { get; set; } = new List<RecipeItem>();
    }

    public class RecipeItem
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Val")]
        public string Val { get; set; }
    }
}
