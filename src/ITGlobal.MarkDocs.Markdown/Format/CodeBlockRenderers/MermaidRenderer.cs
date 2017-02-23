using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.CodeBlockRenderers
{
    internal sealed class MermaidRenderer : ICodeBlockRenderer
    {
        public bool Write(HtmlRenderer renderer, FencedCodeBlock block)
        {
            var infoPrefix = (block.Parser as FencedCodeBlockParser)?.InfoPrefix ??
                             FencedCodeBlockParser.DefaultInfoPrefix;

            renderer.Write("<div")
                .WriteAttributes(block.TryGetAttributes(),
                    cls => cls.StartsWith(infoPrefix) ? cls.Substring(infoPrefix.Length) : cls)
                .Write(">");
            renderer.WriteLeafRawLines(block, true, true, true);
            renderer.WriteLine("</div>");

            return true;
        }
    }
}