using JetBrains.Annotations;
using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Cache.Model
{
    /// <summary>
    ///     Page anchor model
    /// </summary>
    [PublicAPI]
    public sealed class PageAnchorModel
    {
        /// <summary>
        ///     Anchor ID (href)
        /// </summary>
        [JsonProperty("id")]
        [NotNull]
        public string Id { get; set; }

        /// <summary>
        ///     Anchor title
        /// </summary>
        [JsonProperty("title")]
        [NotNull]
        public string Title { get; set; }

        /// <summary>
        ///     Nested anchors
        /// </summary>
        [JsonProperty("nested", NullValueHandling = NullValueHandling.Ignore)]
        [CanBeNull]
        public PageAnchorModel[] Nested { get; set; }
    }
}