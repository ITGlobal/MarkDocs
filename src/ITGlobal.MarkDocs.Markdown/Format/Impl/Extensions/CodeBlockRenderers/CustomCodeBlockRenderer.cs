using System;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.CodeBlockRenderers
{
    internal sealed class CustomCodeBlockRenderer : CodeBlockRenderer
    {
        private readonly CodeBlockRendererSelector _selector;

        public CustomCodeBlockRenderer(CodeBlockRendererSelector selector)
        {
            _selector = selector;
        }
        
        protected override void Write(HtmlRenderer renderer, CodeBlock obj)
        {
            if (!MarkdownRenderingContext.IsPresent)
            {
                return;
            }
            
            renderer.EnsureLine();

            if (obj is FencedCodeBlock codeBlock)
            {
                var r = _selector.TrySelect(MarkdownRenderingContext.RenderContext, codeBlock);
                if (r != null)
                {
                    try
                    {
                        r.Render(MarkdownRenderingContext.RenderContext, renderer, codeBlock);
                        return;
                    }
                    catch
                    {
                        MarkdownRenderingContext.RenderContext.Error("Error while rendering code block", obj.Line);
                    }
                }
            }

            renderer.Write("<pre");
            if (OutputAttributesOnPre)
            {
                renderer.WriteAttributes(obj);
            }
            renderer.Write("><code");
            if (!OutputAttributesOnPre)
            {
                renderer.WriteAttributes(obj);
            }
            renderer.Write(">");
            renderer.WriteLeafRawLines(obj, true, true);
            renderer.WriteLine("</code></pre>");
        }
    }
}