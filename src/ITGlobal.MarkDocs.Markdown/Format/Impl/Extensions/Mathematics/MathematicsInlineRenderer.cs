using System;
using Markdig.Extensions.Mathematics;
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.Mathematics
{
    internal sealed class MathematicsInlineRenderer : HtmlObjectRenderer<MathInline>
    {
        private readonly IMathRenderer _renderer;

        public MathematicsInlineRenderer(IMathRenderer renderer)
        {
            _renderer = renderer;
        }

        protected override void Write(HtmlRenderer renderer, MathInline block)
        {
            if (!MarkdownRenderingContext.IsPresent)
            {
                return;
            }

            var context = MarkdownRenderingContext.RenderContext;
            try
            {
                

                var markup = block.GetText();
                var illustration = context.CreateAttachment(markup, _renderer.Render(markup, block.Line));
                var resourceUrl = MarkdownRenderingContext.ResourceUrlResolver.ResolveUrl(
                    context,
                    illustration.Asset
                );

                renderer.EnsureLine();
                renderer.Write("<img src=\"");
                renderer.WriteEscapeUrl(resourceUrl);
                renderer.Write("\"");
                renderer.WriteAttributes(block);
                renderer.Write(" alt=\"\"");
                renderer.Write(" />");
            }
            catch (Exception e)
            {
                context.Error($"Failed to render math inline. {e.Message}", block.Line);
                renderer.WriteErrorInline("Failed to render inline math block");
            }
        }
    }
}