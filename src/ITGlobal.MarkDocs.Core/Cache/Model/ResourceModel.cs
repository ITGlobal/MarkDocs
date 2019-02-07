using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Cache.Model
{
    public abstract class ResourceModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("filename")]
        public string RelativePath { get; set; }
    }
}