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
        ///     Copies page metadata from <paramref name="source"/>
        /// </summary>
        internal void CopyFrom([NotNull] Metadata source)
        {
            Title = !string.IsNullOrEmpty(source.Title) ? source.Title : Title;
            Order = source.Order != 0 ? source.Order : Order;
            Tags = (source.Tags ?? EmptyTags).Concat(Tags ?? EmptyTags).Distinct().OrderBy(_ => _).ToArray();
        }
    }
}