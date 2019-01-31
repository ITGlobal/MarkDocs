using ITGlobal.MarkDocs.Source;
using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Cache.Model
{
    public sealed class PageModel : ResourceModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("desc")]
        public string Description { get; set; }

        [JsonProperty("metadata")]
        public PageMetadata Metadata { get; set; }

        [JsonProperty("anchors", NullValueHandling = NullValueHandling.Ignore)]
        public PageAnchorModel[] Anchors { get; set; }

        [JsonProperty("preview", NullValueHandling = NullValueHandling.Ignore)]
        public PagePreviewModel Preview { get; set; }

        [JsonProperty("pages", NullValueHandling = NullValueHandling.Ignore)]
        public PageModel[] Pages { get; set; }
    }
}