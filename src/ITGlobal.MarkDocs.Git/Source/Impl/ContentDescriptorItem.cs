using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Source.Impl
{
    internal sealed class ContentDescriptorItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }
    }
}