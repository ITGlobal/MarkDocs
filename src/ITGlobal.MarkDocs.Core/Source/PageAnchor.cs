using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Source
{
    public sealed class PageAnchor
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("nested", NullValueHandling = NullValueHandling.Ignore)]
        public PageAnchor[] Nested { get; set; }
    }
}