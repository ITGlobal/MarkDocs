using ITGlobal.MarkDocs.Source;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Cache.Model
{
    /// <summary>
    ///     A page model
    /// </summary>
    [PublicAPI]
    public sealed class PageModel : ResourceModel
    {
        /// <summary>
        ///     Page title
        /// </summary>
        [JsonProperty("title")]
        [NotNull]
        public string Title { get; set; }

        /// <summary>
        ///     Page description
        /// </summary>
        [JsonProperty("desc")]
        [NotNull]
        public string Description { get; set; }

        /// <summary>
        ///     Page metadata
        /// </summary>
        [JsonProperty("metadata")]
        [NotNull]
        public PageMetadata Metadata { get; set; }

        /// <summary>
        ///     Page anchor tree
        /// </summary>
        [JsonProperty("anchors", NullValueHandling = NullValueHandling.Ignore)]
        [CanBeNull]
        public PageAnchorModel[] Anchors { get; set; }

        /// <summary>
        ///     A page preview model
        /// </summary>
        [JsonProperty("preview", NullValueHandling = NullValueHandling.Ignore)]
        [CanBeNull]
        public PagePreviewModel Preview { get; set; }

        /// <summary>
        ///     Nested pages
        /// </summary>
        [JsonProperty("pages", NullValueHandling = NullValueHandling.Ignore)]
        [CanBeNull]
        public PageModel[] Pages { get; set; }
    }
}