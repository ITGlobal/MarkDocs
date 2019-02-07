using Markdig.Extensions.Mathematics;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.Mathematics
{
    internal sealed class CustomMathInlineRenderer : HtmlMathInlineRenderer
    {
        protected override void Write(HtmlRenderer renderer, MathInline inline)
        {
            if (MarkdownPageRenderContext.IsPresent)
            {
                var r = inline.GetCustomRenderable();
                if (r != null)
                {
                    var context = MarkdownPageRenderContext.Current;
                    try
                    {
                        r.Render(context, renderer);
                        return;
                    }
                    catch
                    {
                        context.Error("Error while rendering math inline", inline.Line);
                    }
                }
            }

            base.Write(renderer, inline);
        }
    }
}