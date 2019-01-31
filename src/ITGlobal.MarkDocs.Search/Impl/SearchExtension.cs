using ITGlobal.MarkDocs.Extensions;

namespace ITGlobal.MarkDocs.Search.Impl
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
        public SearchExtension(LuceneSearchEngine engine)
        {
            _engine = engine;
        }

        #endregion

        #region IExtension

        public void Initialize(IMarkDocState state) => _engine.InitializeIndex(state);

        public void OnCreated(IDocumentation documentation) => _engine.AddIndex(documentation);

        public void OnUpdated(IDocumentation documentation) => _engine.BeginUpdateIndex(documentation);

        public void OnUpdateCompleted(IDocumentation documentation) => _engine.EndUpdateIndex(documentation);

        public void OnRemoved(IDocumentation documentation) => _engine.DropIndex(documentation);

        #endregion

        #region public methods

        public ISearchService GetSearchService(IDocumentation documentation)
            => new SearchServiceWrapper(documentation, _engine);

        #endregion
    }
}
