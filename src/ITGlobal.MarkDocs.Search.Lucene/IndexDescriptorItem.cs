using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Search
{
    internal sealed class IndexDescriptorItem
    {
        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }
}