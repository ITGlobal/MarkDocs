using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax.Inlines;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.Images
{
    internal sealed class CustomLinkInlineRenderer : LinkInlineRenderer
    {
        protected override void Write(HtmlRenderer renderer, LinkInline link)
        {
            if (!MarkdownPageRenderContext.IsPresent)
            {
                base.Write(renderer, link);
                return;
            }

            var context = MarkdownPageRenderContext.Current;

            var r = link.GetCustomRenderable();
            if (r != null)
            {
                try
                {
                    r.Render(context, renderer);
                    return;
                }
                catch
                {
                    context.Error("Error while rendering link inline", link.Line);
                }
            }

            base.Write(renderer, link);
        }
    }
}