using System;
using System.IO;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Format
{
    [PublicAPI]
    public sealed class HighlightJsOptions
    {
        public HighlightJsStylesheet Stylesheet { get; set; } = HighlightJsStylesheet.Vs;

        public string TempDirectory { get; set; } = Path.Combine(Path.GetTempPath(), $"markdocs-hljs-{Guid.NewGuid():N}");
    }
}