using System.Linq;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.TableOfContents
{
    internal sealed class TableOfContentsBlockRenderer : HtmlObjectRenderer<TableOfContentsBlock>
    {
        protected override void Write(HtmlRenderer renderer, TableOfContentsBlock block)
        {
            var document = block.Document;
            
            var headers = document.OfType<HeadingBlock>().ToArray();

            renderer.WriteLine("<div class=\"panel panel-primary\">");
            renderer.WriteLine("  <div class=\"panel-heading\">Contents</div>");
            renderer.WriteLine("  <div class=\"list-group\">");
            foreach (var header in headers)
            {
                var id = header.GetAttributes().Id;
                var text = header.Inline.GetText();
                
                renderer.Write($"   <a href=\"#{id}\" class=\"list-group-item\" style=\"margin-left: {header.Level * 10}px\">{text}</a>");
            }
            renderer.WriteLine("  </div>");
            renderer.WriteLine("</div>");
        }
    }
}