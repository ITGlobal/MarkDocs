using Markdig.Renderers;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.HighlightJs
{
    internal sealed class ClientSideHighlightJsRenderable : IRenderable
    {
        private readonly FencedCodeBlock _block;

        public ClientSideHighlightJsRenderable(FencedCodeBlock block)
        {
            _block = block;
        }

        public void Render(IPageRenderContext ctx, HtmlRenderer renderer)
        {
            var language = _block.Info;

            renderer.Write("<pre>");
            renderer.Write($"<code class=\"{language}\">");
            renderer.WriteLeafRawLines(_block, true, true);
            renderer.WriteLine("</code></pre>");
        }
    }
}