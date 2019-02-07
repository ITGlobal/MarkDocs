using JetBrains.Annotations;
using Markdig.Extensions.Mathematics;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     A MathML/Tex/LaTex renderer
    /// </summary>
    [PublicAPI]
    public interface IMathRenderer
    {
        /// <summary>
        ///     Creates a renderable object for specified markup node
        /// </summary>
        [NotNull]
        IRenderable CreateRenderable([NotNull] IPageReadContext ctx, [NotNull] MathBlock block);

        /// <summary>
        ///     Creates a renderable object for specified markup node
        /// </summary>
        [NotNull]
        IRenderable CreateRenderable([NotNull] IPageReadContext ctx, [NotNull] MathInline block);
    }
}