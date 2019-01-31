using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.Admonition
{
    internal sealed class AdmonitionRenderer : HtmlObjectRenderer<AdmonitionBlock>
    {
        protected override void Write(HtmlRenderer renderer, AdmonitionBlock block)
        {
            renderer.EnsureLine();
            renderer.WriteLine($"<p class=\"alert alert-{block.Type}\">");

            if (!string.IsNullOrWhiteSpace(block.Title))
            {
                renderer.Write("<strong>");
                renderer.WriteEscape(block.Title);
                renderer.WriteLine("</strong>");
            }

            renderer.WriteLeafInline(block);
            renderer.WriteLine("</p>");
        }
    }
}