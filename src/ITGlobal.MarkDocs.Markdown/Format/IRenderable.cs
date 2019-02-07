using JetBrains.Annotations;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Format
{
    public interface IRenderable
    {
        void Render([NotNull] IPageRenderContext ctx, [NotNull] HtmlRenderer renderer);
    }
}