using Markdig.Extensions.Mathematics;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.Mathematics
{
    internal sealed class CustomMathBlockRenderer : HtmlMathBlockRenderer
    {
        protected override void Write(HtmlRenderer renderer, MathBlock block)
        {
            if (MarkdownPageRenderContext.IsPresent)
            {
                var r = block.GetCustomRenderable();
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
                        context.Error("Error while rendering math block", block.Line);
                    }
                }
            }

            base.Write(renderer, block);
        }
    }
}