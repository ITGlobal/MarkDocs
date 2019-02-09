using System;
using System.IO;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     Options for highlight.js-based code block renderer
    /// </summary>
    [PublicAPI]
    public sealed class HighlightJsOptions
    {
        /// <summary>
        ///     CSS style for code highlight
        /// </summary>
        public HighlightJsStylesheet Stylesheet { get; set; } = HighlightJsStylesheet.Vs;

        /// <summary>
        ///     Path to temp directory for server side JS worker
        /// </summary>
        public string TempDirectory { get; set; } = Path.Combine(Path.GetTempPath(), $"markdocs-hljs-{Guid.NewGuid():N}");
    }
}