using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.CodeBlockRenderers
{
    internal sealed class MermaidRenderer : ICodeBlockRenderer
    {
        public const string Language = "mermaid";
        
        public bool CanRender(IPageRenderContext ctx, FencedCodeBlock block)
        {
            return block.Info == Language;
        }

        public void Render(IPageRenderContext ctx, HtmlRenderer renderer, FencedCodeBlock block)
        {
            var infoPrefix = (block.Parser as FencedCodeBlockParser)?.InfoPrefix ??
                             FencedCodeBlockParser.DefaultInfoPrefix;

            renderer.Write("<div")
                .WriteAttributes(block.TryGetAttributes(),
                    cls => cls.StartsWith(infoPrefix) ? cls.Substring(infoPrefix.Length) : cls)
                .Write(">");
            renderer.WriteLeafRawLines(block, true, true, true);
            renderer.WriteLine("</div>");
        }
    }
}