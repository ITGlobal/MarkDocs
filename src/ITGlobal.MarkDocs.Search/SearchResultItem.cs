using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Search
{
    /// <summary>
    ///     A search result item
    /// </summary>
    [PublicAPI]
    public sealed class SearchResultItem
    {
        /// <summary>
        ///     .ctor
        /// </summary>
        [PublicAPI]
        public SearchResultItem(IPage page, string preview)
        {
            Page = page;
            Preview = preview;
        }

        /// <summary>
        ///     Page
        /// </summary>
        [PublicAPI, NotNull]
        public IPage Page { get; }

        /// <summary>
        ///     Preview (markup)
        /// </summary>
        [PublicAPI, NotNull]
        public string Preview { get; }
    }
}