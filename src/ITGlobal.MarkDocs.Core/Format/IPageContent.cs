using ITGlobal.MarkDocs.Source;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     A page asset content
    /// </summary>
    [PublicAPI]
    public interface IPageContent
    {
        /// <summary>
        ///     Anchors
        /// </summary>
        [CanBeNull]
        PageAnchors Anchors { get; }

        /// <summary>
        ///     true if page contains a "cut" separator
        /// </summary>
        bool HasPreview { get; }

        /// <summary>
        ///     Renders page into HTML
        /// </summary>
        [NotNull]
        string Render([NotNull] IPageRenderContext ctx);

        /// <summary>
        ///     Renders page preview into HTML
        /// </summary>
        [NotNull]
        string RenderPreview([NotNull] IPageRenderContext ctx);
    }
}