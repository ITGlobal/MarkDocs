using System.Collections.Generic;
using System.Linq;

namespace ITGlobal.MarkDocs.Blog.Impl
{
    /// <summary>
    ///     Blog post search result (by text)
    /// </summary>
    internal sealed class TextSearchResult : ITextSearchResult
    {
        private readonly IReadOnlyList<ITextSearchResultItem> _posts;

        public TextSearchResult(IReadOnlyList<ITextSearchResultItem> posts, string query)
        {
            _posts = posts;
            Query = query;
        }

        /// <summary>
        ///     Search query
        /// </summary>
        public string Query { get; }

        /// <summary>
        ///     Gets blog post total count
        /// </summary>
        public int Count => _posts.Count;

        /// <summary>
        ///     Gets a blog posts paged list
        /// </summary>
        public IReadOnlyList<ITextSearchResultItem> List(int page, int pageSize = BlogEngineConstants.PageSize)
            => _posts.Skip(page * pageSize).Take(pageSize).ToArray();
    }
}