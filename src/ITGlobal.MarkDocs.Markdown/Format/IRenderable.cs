using JetBrains.Annotations;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     Custom object renderer
    /// </summary>
    [PublicAPI]
    public interface IRenderable
    {
        /// <summary>
        ///     Renders itself into HTML
        /// </summary>
        void Render([NotNull] IPageRenderContext ctx, [NotNull] HtmlRenderer renderer);
    }
}