using Markdig.Renderers;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.CodeBlockRenderers
{
    internal abstract class PlantUmlRenderer : ICodeBlockRenderer
    {
        public const string Language = "plantuml";

        public bool CanRender(IPageRenderContext ctx, FencedCodeBlock block)
            => MarkdownRenderingContext.IsPresent && block.Info == Language;

        public void Render(IPageRenderContext ctx, HtmlRenderer renderer, FencedCodeBlock block)
        {
            if (!MarkdownRenderingContext.IsPresent)
            {
                return;
            }

            var markup = block.GetText();
            var context = MarkdownRenderingContext.RenderContext;
            var result = context.CreateAttachment(markup, GenerateContent(markup, block.Line));
            var resourceUrl = MarkdownRenderingContext.ResourceUrlResolver.ResolveUrl(context, result.Asset);

            renderer.Write("<img src=\"");
            renderer.WriteEscapeUrl(resourceUrl);
            renderer.Write("\"");
            renderer.WriteAttributes(block);
            renderer.Write(" alt=\"\"");
            renderer.Write(" />");
        }

        protected abstract IGeneratedAssetContent GenerateContent(string source, int? lineNumber);
    }
}