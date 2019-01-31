using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Search.Impl
{
    internal sealed class IndexDescriptorItem
    {
        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }
}
