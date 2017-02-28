using Markdig.Renderers;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.CodeBlockRenderers
{
    internal sealed class SourceCodeRenderer : ICodeBlockRenderer
    {
        private readonly ISyntaxColorizer _syntaxColorizer;

        public SourceCodeRenderer(ISyntaxColorizer syntaxColorizer)
        {
            _syntaxColorizer = syntaxColorizer;
        }

        public bool Write(HtmlRenderer renderer, FencedCodeBlock block)
        {
            var markup = block.GetText();
            var html = _syntaxColorizer.Render(block.Info, markup);
            
            renderer.WriteLine("<div>");
            renderer.WriteLine(html);
            renderer.WriteLine("</div>");

            return true;
        }
    }
}