using JetBrains.Annotations;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     Custom renderer for <typeparamref name="T"/> markdown blocks
    /// </summary>
    [PublicAPI]
    public interface IRenderer<T>
        where T: MarkdownObject
    {
        /// <summary>
        ///     Creates a renderable object for specified markup node
        /// </summary>
        [CanBeNull]
        IRenderable TryCreateRenderable([NotNull] IPageReadContext ctx, [NotNull] T obj);
    }
}