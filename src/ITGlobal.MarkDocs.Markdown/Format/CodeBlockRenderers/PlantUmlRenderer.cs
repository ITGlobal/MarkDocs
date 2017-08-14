using System;
using Markdig.Renderers;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.CodeBlockRenderers
{
    internal sealed class PlantUmlRenderer : ICodeBlockRenderer
    {
        public bool Write(HtmlRenderer renderer, FencedCodeBlock block)
        {
            if (!MarkdownRenderingContext.IsPresent || MarkdownRenderingContext.UmlRenderer == null)
            {
                return false;
            }

            var markup = block.GetText();
            var image = MarkdownRenderingContext.UmlRenderer.Render(markup, block.Line);
            var filename = $"uml_{Guid.NewGuid():N}{image.FileType}";

            var illustration = MarkdownRenderingContext.RenderContext.CreateAttachment(filename, image.Content);

            var resourceUrl = MarkdownRenderingContext.ResourceUrlResolver.ResolveUrl(illustration, MarkdownRenderingContext.RenderContext.Page);

            renderer.Write("<img src=\"");
            renderer.WriteEscapeUrl(resourceUrl);
            renderer.Write("\"");
            renderer.WriteAttributes(block);
            renderer.Write(" alt=\"\"");
            renderer.Write(" />");

            return true;
        }
    }
}