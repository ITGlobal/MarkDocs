using JetBrains.Annotations;
using Markdig.Syntax.Inlines;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     Renders images
    /// </summary>
    [PublicAPI]
    public interface IImageRenderer : IRenderer<LinkInline> { }
}