using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Git
{
    internal sealed class ContentDescriptorItem
    {
        [JsonProperty("path")]
        public string Path { get; set; }
    }
}