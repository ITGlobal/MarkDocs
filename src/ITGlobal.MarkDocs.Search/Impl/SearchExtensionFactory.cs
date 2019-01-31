using ITGlobal.MarkDocs.Extensions;

namespace ITGlobal.MarkDocs.Search.Impl
{
    /// <summary>
    ///     A factory for search extension
    /// </summary>
    internal sealed class SearchExtensionFactory : IExtensionFactory
    {
        private readonly LuceneSearchEngine _engine;

        public SearchExtensionFactory(LuceneSearchEngine engine)
        {
            _engine = engine;
        }

        public IExtension Create(IMarkDocService service) => new SearchExtension(_engine);
    }
}
