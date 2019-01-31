using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Cache.Model
{
    public sealed class PageAnchorModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("nested", NullValueHandling = NullValueHandling.Ignore)]
        public PageAnchorModel[] Nested { get; set; }
    }
}