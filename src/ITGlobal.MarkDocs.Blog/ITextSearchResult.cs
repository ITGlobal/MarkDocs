using JetBrains.Annotations;
using System.Collections.Generic;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Blog post search result (by text)
    /// </summary>
    [PublicAPI]
    public interface ITextSearchResult
    {
        /// <summary>
        ///     Search query
        /// </summary>
        [NotNull]
        string Query { get; }

        /// <summary>
        ///     Gets search resultscount
        /// </summary>
        int Count { get; }

        /// <summary>
        ///     Gets a blog posts paged list
        /// </summary>
        [NotNull]
        IReadOnlyList<ITextSearchResultItem> List(int page, int pageSize = BlogEngineConstants.PageSize);
    }
}
