using ITGlobal.MarkDocs.Source;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.ChildrenList
{
    internal sealed class DefaultChildrenListRenderer : IChildrenListRenderer
    {
        private sealed class ChildPageIResourceUrlResolutionContext : IResourceUrlResolutionContext
        {
            private readonly IPageRenderContext _context;

            public ChildPageIResourceUrlResolutionContext(IPageRenderContext context)
            {
                _context = context;
            }

            public string SourceTreeId => _context.AssetTree.Id;
            public IResourceId Page => _context.Page;
            public bool IsBranchPage => _context.Page is BranchPageAsset;
        }

        private readonly IResourceUrlResolver _resolver;

        public DefaultChildrenListRenderer(IResourceUrlResolver resolver)
        {
            _resolver = resolver;
        }

        public void Render(HtmlRenderer renderer, AssetTree assetTree, PageAsset page)
        {
            if (!MarkdownPageRenderContext.IsPresent)
            {
                return;
            }

            var context = MarkdownPageRenderContext.Current;

            var parentPage = assetTree.TryGetParentPage(page.Id);
            if (parentPage == null)
            {
                return;
            }

            renderer.WriteLine("<ul>");

            foreach (var p in parentPage.Subpages)
            {
                var url = _resolver.ResolveUrl(new ChildPageIResourceUrlResolutionContext(context), p);

                renderer.WriteLine("<li>");
                renderer.Write($"<a href=\"{url}\">{p.Metadata.Title}</a>");
                renderer.WriteLine("</li>");
            }
            renderer.WriteLine("</ul>");
        }
    }
}