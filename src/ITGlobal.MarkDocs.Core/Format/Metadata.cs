using System.Linq;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     Page metadata
    /// </summary>
    [PublicAPI]
    public sealed class Metadata
    {
        private static readonly string[] EmptyTags = new string[0];
        private static readonly MetaTag[] EmptyMetaTags = new MetaTag[0];

        /// <summary>
        ///     Page permanent identifier
        /// </summary>
        [PublicAPI]
        public string ContentId { get; set; }

        /// <summary>
        ///     Page title
        /// </summary>
        [PublicAPI]
        public string Title { get; set; }

        /// <summary>
        ///     Page order
        /// </summary>
        [PublicAPI]
        public int Order { get; set; }

        /// <summary>
        ///     Page tags
        /// </summary>
        [PublicAPI]
        public string[] Tags { get; set; } = EmptyTags;

        /// <summary>
        ///     Page meta tags
        /// </summary>
        [PublicAPI]
        public MetaTag[] MetaTags { get; set; } = EmptyMetaTags;

        /// <summary>
        ///     Copies page metadata from <paramref name="source"/>
        /// </summary>
        internal void CopyFrom([NotNull] Metadata source)
        {
            ContentId = !string.IsNullOrEmpty(source.ContentId) ? source.ContentId : ContentId;
            Title = !string.IsNullOrEmpty(source.Title) ? source.Title : Title;
            Order = source.Order != 0 ? source.Order : Order;
            Tags = (source.Tags ?? EmptyTags).Concat(Tags ?? EmptyTags).Distinct().OrderBy(_ => _).ToArray();
            MetaTags = (source.MetaTags ?? EmptyMetaTags).Concat(MetaTags ?? EmptyMetaTags).Distinct().OrderBy(_ => _.Name).ToArray();
        }
    }
}