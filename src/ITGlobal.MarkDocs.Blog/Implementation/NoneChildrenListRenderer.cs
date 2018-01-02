using ITGlobal.MarkDocs.Format;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Blog.Implementation
{
    internal sealed class NoneChildrenListRenderer : IChildrenListRenderer
    {
        public void Render(HtmlRenderer renderer, IPage page) { }
    }
}
