using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     A MathML/Tex/LaTex renderer
    /// </summary>
    [PublicAPI]
    public interface IMathRenderer
    {
        /// <summary>
        ///     Render a MathML/Tex/LaTex into an image
        /// </summary>
        [PublicAPI, NotNull]
        ImageData Render([NotNull] string sourceCode);
    }
}