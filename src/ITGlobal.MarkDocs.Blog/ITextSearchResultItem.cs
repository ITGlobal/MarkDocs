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
        [NotNull]
        IBlogPost Post { get; }

        /// <summary>
        ///     Preview (HTML)
        /// </summary>
        [NotNull]
        string Preview { get; }
    }
}