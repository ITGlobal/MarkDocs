using ITGlobal.MarkDocs.Source;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     A page that has been parsed already
    /// </summary>
    [PublicAPI]
    public interface IParsedPage
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