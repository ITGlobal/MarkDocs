using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Blog.Impl
{
    internal sealed class BlogRootPageContent : IPageContent
    {
        public PageAnchors Anchors => null;
        public bool HasPreview => false;
        public string Render(IPageRenderContext ctx) => "";
        public string RenderPreview(IPageRenderContext ctx) => "";
    }
}