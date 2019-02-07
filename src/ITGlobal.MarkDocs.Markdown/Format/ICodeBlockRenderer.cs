using JetBrains.Annotations;
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
        bool CanRender([NotNull] IPageReadContext ctx, [NotNull] FencedCodeBlock block);

        /// <summary>
        ///     Creates a renderable object for specified markup node
        /// </summary>
        [NotNull]
        IRenderable CreateRenderable([NotNull] IPageReadContext ctx, [NotNull] FencedCodeBlock block);
    }
}