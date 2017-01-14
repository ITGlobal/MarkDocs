using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace ITGlobal.MarkDocs.Format.Icons
{
    internal sealed class IconInlineRenderer : HtmlObjectRenderer<IconInline>
    {
        protected override void Write(HtmlRenderer renderer, IconInline obj)
        {
            if (renderer.EnableHtmlForInline)
            {
                renderer.Write("<i class=\"")
                    .WriteEscape(obj.Type)
                    .Write(" ")
                    .WriteEscape(obj.Id)
                    .Write("\"></i>");
            }
        }
    }
}