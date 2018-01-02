using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     A text search result item
    /// </summary>
    [PublicAPI]
    public interface ITextSearchResultItem
    {
        /// <summary>
        ///     Blog post
        /// </summary>
        [PublicAPI, NotNull]
        IBlogPost Post{ get; }

        /// <summary>
        ///     Preview (HTML)
        /// </summary>
        [PublicAPI, NotNull]
        string Preview { get; }
    }
}