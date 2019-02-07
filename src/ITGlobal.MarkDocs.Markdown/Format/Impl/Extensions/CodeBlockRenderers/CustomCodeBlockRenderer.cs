using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.CodeBlockRenderers
{
    internal sealed class CustomCodeBlockRenderer : CodeBlockRenderer
    {
        protected override void Write(HtmlRenderer renderer, CodeBlock obj)
        {
            if (!MarkdownPageRenderContext.IsPresent)
            {
                base.Write(renderer, obj);
                return;
            }

            var context = MarkdownPageRenderContext.Current;
            if (obj is FencedCodeBlock codeBlock)
            {
                var r = codeBlock.GetCustomRenderable();
                if (r != null)
                {
                    try
                    {
                        r.Render(context, renderer);
                        return;
                    }
                    catch
                    {
                        context.Error("Error while rendering code block", obj.Line);
                    }
                }
            }

            base.Write(renderer, obj);
        }
    }
}