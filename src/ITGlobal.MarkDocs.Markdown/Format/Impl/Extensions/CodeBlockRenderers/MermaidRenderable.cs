using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.CodeBlockRenderers
{
    internal sealed class MermaidRenderable : IRenderable
    {
        private readonly FencedCodeBlock _block;

        public MermaidRenderable(FencedCodeBlock block)
        {
            _block = block;
        }

        public void Render(IPageRenderContext ctx, HtmlRenderer renderer)
        {
            var infoPrefix = (_block.Parser as FencedCodeBlockParser)?.InfoPrefix ??
                             FencedCodeBlockParser.DefaultInfoPrefix;

            renderer.Write("<div")
                .WriteAttributes(_block.TryGetAttributes(),
                    cls => cls.StartsWith(infoPrefix) ? cls.Substring(infoPrefix.Length) : cls)
                .Write(">");
            renderer.WriteLeafRawLines(_block, true, true, true);
            renderer.WriteLine("</div>");
        }
    }
}