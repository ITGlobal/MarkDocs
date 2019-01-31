using System;
using Markdig.Extensions.Mathematics;
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.Mathematics
{
    internal sealed class MathematicsBlockRenderer : HtmlObjectRenderer<MathBlock>
    {
        private readonly IMathRenderer _renderer;

        public MathematicsBlockRenderer(IMathRenderer renderer)
        {
            _renderer = renderer;
        }

        protected override void Write(HtmlRenderer renderer, MathBlock block)
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
                context.Error($"Failed to render math block. {e.Message}", block.Line);
                renderer.WriteError("Failed to render math block");
            }
        }
    }
}