using System.Collections.Generic;

namespace ITGlobal.MarkDocs.Search.Impl
{
    /// <summary>
    ///     Search service wrapper class
    /// </summary>
    internal sealed class SearchServiceWrapper : ISearchService
    {
        private readonly IDocumentation _documentation;
        private readonly LuceneSearchEngine _engine;

        public SearchServiceWrapper(IDocumentation documentation, LuceneSearchEngine engine)
        {
            _documentation = documentation;
            _engine = engine;
        }

        /// <summary>
        ///     Search documentation
        /// </summary>
        /// <param name="query">
        ///     Search query
        /// </param>
        /// <returns>
        ///     Search results
        /// </returns>
        public IReadOnlyList<SearchResultItem> Search(string query)
            => _engine.Search(_documentation, query);

        /// <summary>
        ///     Gets a list of search query suggestions
        /// </summary>
        /// <param name="query">
        ///     Search query
        /// </param>
        /// <returns>
        ///     List of search suggestions
        /// </returns>
        public IReadOnlyList<string> Suggest(string query)
            => _engine.Suggest(_documentation, query);
    }
}
