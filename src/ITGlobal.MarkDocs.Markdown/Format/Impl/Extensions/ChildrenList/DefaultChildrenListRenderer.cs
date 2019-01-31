using ITGlobal.MarkDocs.Source;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.ChildrenList
{
    internal sealed class DefaultChildrenListRenderer : IChildrenListRenderer
    {
        private readonly IResourceUrlResolver _resolver;

        public DefaultChildrenListRenderer(IResourceUrlResolver resolver)
        {
            _resolver = resolver;
        }

        public void Render(HtmlRenderer renderer, AssetTree assetTree, PageAsset page)
        {
            if (!MarkdownRenderingContext.IsPresent)
            {
                return;
            }

            var context = MarkdownRenderingContext.RenderContext;

            var parentPage = assetTree.TryGetParentPage(page.Id);
            if (parentPage == null)
            {
                return;
            }

            renderer.WriteLine("<ul>");

            foreach (var p in parentPage.Subpages)
            {
                var url = _resolver.ResolveUrl(context, p);

                renderer.WriteLine("<li>");
                renderer.Write($"<a href=\"{url}\">{p.Metadata.Title}</a>");
                renderer.WriteLine("</li>");
            }
            renderer.WriteLine("</ul>");
        }
    }
}