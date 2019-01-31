using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.Alerts
{
    internal class AlertBlockRenderer : HtmlObjectRenderer<AlertBlock>
    {
        protected override void Write(HtmlRenderer renderer, AlertBlock block)
        {
            renderer.EnsureLine();
            renderer.Write("<div class=\"alert ");
            switch (block.Type)
            {
                case AlertBlockType.Information:
                    renderer.WriteLine("alert-info\">");
                    break;
                case AlertBlockType.Warning:
                    renderer.WriteLine("alert-danger\">");
                    break;
                default:
                    renderer.WriteLine("\">");
                    break;
            }
            
            var savedImplicitParagraph = renderer.ImplicitParagraph;
            renderer.ImplicitParagraph = false;
            renderer.WriteChildren(block);
            renderer.ImplicitParagraph = savedImplicitParagraph;
            renderer.WriteLine("</div>");
        }
    }
}