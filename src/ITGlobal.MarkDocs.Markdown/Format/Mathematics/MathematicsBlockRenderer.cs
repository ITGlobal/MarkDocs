using System;
using Markdig.Extensions.Mathematics;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Microsoft.Extensions.Logging;
using static ITGlobal.MarkDocs.Format.MarkdownRenderingContext;

namespace ITGlobal.MarkDocs.Format.Mathematics
{
    internal sealed class MathematicsBlockRenderer : HtmlObjectRenderer<MathBlock>
    {
        protected override void Write(HtmlRenderer renderer, MathBlock block)
        {
            try
            {
                if (!IsMarkdownRenderingContextPresent || MathRenderer == null)
                {
                    return;
                }

                var markup = block.GetText();
                var image = MathRenderer.Render(markup);
                var filename = $"uml_{Guid.NewGuid():N}{image.FileType}";

                var illustration = RenderContext.CreateAttachment(filename, image.Content);

                var resourceUrl = ResourceUrlResolver.ResolveUrl(illustration, RenderContext.Page);

                renderer.EnsureLine();
                renderer.Write("<img src=\"");
                renderer.WriteEscapeUrl(resourceUrl);
                renderer.Write("\"");
                renderer.WriteAttributes(block);
                renderer.Write(" alt=\"\"");
                renderer.Write(" />");
            }
            catch (Exception exception)
            {
                Logger.LogError(0, exception, "Error while rendering {0}", nameof(MathBlock));
                renderer.WriteError("Failed to render math block");
            }
        }
    }
}