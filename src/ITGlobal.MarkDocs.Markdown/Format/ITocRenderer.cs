using System.Collections.Generic;
using ITGlobal.MarkDocs.Source;
using JetBrains.Annotations;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     HTML renderer for TOC
    /// </summary>
    [PublicAPI]
    public interface ITocRenderer
    {
        /// <summary>
        ///     Renders a TOC into HTML
        /// </summary>
        void Render(HtmlRenderer renderer, IReadOnlyList<PageAnchor> anchors);
    }
}