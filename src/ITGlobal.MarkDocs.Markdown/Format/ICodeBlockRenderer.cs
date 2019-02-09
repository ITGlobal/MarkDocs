using JetBrains.Annotations;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     Renders fenced code blocks
    /// </summary>
    [PublicAPI]
    public interface ICodeBlockRenderer : IRenderer<FencedCodeBlock> { }
}