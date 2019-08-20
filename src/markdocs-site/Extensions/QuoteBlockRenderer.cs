using Markdig.Renderers;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Site.Extensions
{
    public static class QuoteBlockRenderer
    {
        public static bool Render(HtmlRenderer renderer, QuoteBlock block)
        {
            renderer.EnsureLine();
            renderer.WriteLine("<div class=\"alert alert-primary\">");
            var savedImplicitParagraph = renderer.ImplicitParagraph;
            renderer.ImplicitParagraph = false;
            renderer.WriteChildren(block);
            renderer.ImplicitParagraph = savedImplicitParagraph;

            renderer.WriteLine("</div>");
            renderer.EnsureLine();
            return true;
        }
    }
}