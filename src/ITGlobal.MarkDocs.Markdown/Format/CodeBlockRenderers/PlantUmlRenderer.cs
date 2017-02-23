using System;
using Markdig.Renderers;
using Markdig.Syntax;
using static ITGlobal.MarkDocs.Format.MarkdownRenderingContext;

namespace ITGlobal.MarkDocs.Format.CodeBlockRenderers
{
    internal sealed class PlantUmlRenderer : ICodeBlockRenderer
    {
        public bool Write(HtmlRenderer renderer, FencedCodeBlock block)
        {
            if (!IsMarkdownRenderingContextPresent || UmlRenderer == null)
            {
                return false;
            }
            
            var markup = block.GetText();
            var image = UmlRenderer.Render(markup);
            var filename = $"uml_{Guid.NewGuid():N}{image.FileType}";
            
            var illustration = RenderContext.CreateAttachment(filename, image.Content);

            var resourceUrl = ResourceUrlResolver.ResolveUrl(illustration);

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