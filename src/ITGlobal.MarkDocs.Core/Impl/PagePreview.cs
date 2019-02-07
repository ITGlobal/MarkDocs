using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Cache.Model;
using System.IO;

namespace ITGlobal.MarkDocs.Impl
{
    internal sealed class PagePreview : IPagePreview
    {
        private readonly ICacheReader _cache;
        private readonly IDocumentation _documentation;
        private readonly PagePreviewModel _model;

        public PagePreview(ICacheReader cache, IDocumentation documentation, PagePreviewModel model, IPage page)
        {
            _cache = cache;
            Documentation = documentation;
            _model = model;
            Page = page;
        }

        public string Id => _model.Id;
        public IDocumentation Documentation { get; }
        public string RelativePath => _model.RelativePath;
        public ResourceType Type => ResourceType.Page;
        public IPage Page { get; }

        public Stream OpenRead() => _cache.Read(this);
    }
}