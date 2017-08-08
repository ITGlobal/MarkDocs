using ITGlobal.MarkDocs.Extensions;

namespace ITGlobal.MarkDocs.Search
{

    /// <summary>
    ///     Search service
    /// </summary>
    internal sealed class SearchExtension : IExtension
    {
        #region fields

        private readonly LuceneSearchEngine _engine;

        #endregion

        #region .ctor

        /// <summary>
        ///     .ctor
        /// </summary>
        public SearchExtension(LuceneSearchEngine engine, IMarkDocServiceState state)
        {
            _engine = engine;
            engine.Index(state, isInitial: true);
        }

        #endregion

        #region IExtension

        /// <summary>
        ///     Handle a documentation state update
        /// </summary>
        /// <param name="state">
        ///     New documentation state
        /// </param>
        public void Update(IMarkDocServiceState state) => _engine.Index(state, isInitial: false);

        #endregion

        #region public methods

        public ISearchService GetSearchService(IDocumentation documentation)
            => new SearchServiceWrapper(documentation, _engine);

        #endregion
    }
}
