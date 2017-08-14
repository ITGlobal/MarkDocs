using System;
using Markdig.Extensions.Mathematics;
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace ITGlobal.MarkDocs.Format.Mathematics
{
    internal sealed class MathematicsBlockRenderer : HtmlObjectRenderer<MathBlock>
    {
        protected override void Write(HtmlRenderer renderer, MathBlock block)
        {
            try
            {
                if (!MarkdownRenderingContext.IsPresent || MarkdownRenderingContext.MathRenderer == null)
                {
                    return;
                }

                var markup = block.GetText();
                var image = MarkdownRenderingContext.MathRenderer.Render(markup, block.Line);
                var filename = $"math_{Guid.NewGuid():N}{image.FileType}";

                var illustration = MarkdownRenderingContext.RenderContext.CreateAttachment(filename, image.Content);

                var resourceUrl = MarkdownRenderingContext.ResourceUrlResolver.ResolveUrl(illustration, MarkdownRenderingContext.RenderContext.Page);

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
                MarkdownRenderingContext.RenderContext?.Error($"Failed to render math block. {e.Message}", block.Line, e);
                renderer.WriteError("Failed to render math block");
            }
        }
    }
}