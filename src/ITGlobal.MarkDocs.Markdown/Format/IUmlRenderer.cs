using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     A PlantUML renderer
    /// </summary>
    [PublicAPI]
    public interface IUmlRenderer
    {
        /// <summary>
        ///     Render a PlantUML into an image
        /// </summary>
        [PublicAPI, NotNull]
        ImageData Render([NotNull] string sourceCode);
    }
}