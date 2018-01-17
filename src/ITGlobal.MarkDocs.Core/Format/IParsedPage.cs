using System.Collections.Generic;
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
        ///     true if page contains a "cut" separator
        /// </summary>
        bool HasPreview { get; }

        /// <summary>
        ///     Page anchors (with names)
        /// </summary>
        [NotNull]
        IReadOnlyDictionary<string, string> Anchors { get; }

        /// <summary>
        ///     Renders page into HTML
        /// </summary>
        string Render([NotNull] IRenderContext ctx);

        /// <summary>
        ///     Renders page preview into HTML
        /// </summary>
        string RenderPreview([NotNull] IRenderContext ctx);
    }
}