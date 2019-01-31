using JetBrains.Annotations;
using Markdig.Renderers;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     Renders fenced code blocks
    /// </summary>
    [PublicAPI]
    public interface ICodeBlockRenderer
    {
        /// <summary>
        ///     Checks whether a code block can be rendered wit current renderer
        /// </summary>
        bool CanRender([NotNull] IPageRenderContext ctx, [NotNull] FencedCodeBlock block);

        /// <summary>
        ///     Renders a code block
        /// </summary>
        void Render([NotNull] IPageRenderContext ctx, [NotNull] HtmlRenderer renderer, [NotNull] FencedCodeBlock block);
    }
}