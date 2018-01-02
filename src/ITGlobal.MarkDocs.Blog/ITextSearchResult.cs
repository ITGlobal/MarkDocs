using System.Collections.Generic;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Blog post search result (by text)
    /// </summary>
    public interface ITextSearchResult 
    {
        /// <summary>
        ///     Search query
        /// </summary>
        [PublicAPI, NotNull]
        string Query { get; }

        /// <summary>
        ///     Gets search resultscount
        /// </summary>
        [PublicAPI]
        int Count { get; }

        /// <summary>
        ///     Gets a blog posts paged list
        /// </summary>
        [PublicAPI, NotNull]
        IReadOnlyList<ITextSearchResultItem> List(int page, int pageSize = BlogEngineConstants.PageSize);
    }
}
