using System.Collections.Generic;
using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Source;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Tools.Serve.Extensions
{
    public sealed class CustomTocRenderer : ITocRenderer
    {
        public void Render(HtmlRenderer renderer, IReadOnlyList<PageAnchor> anchors)
        {
            var html = RazorViewRenderer.Render(Startup.ApplicationServices, "~/Serve/Views/Render/TocTree.cshtml", anchors);
            renderer.Write(html);
        }
    }
}