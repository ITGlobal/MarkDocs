using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     Renders HTML blocks
    /// </summary>
    [PublicAPI]
    public interface IHtmlBlockRenderer : IRenderer<ParsedHtmlBlock> { }
}