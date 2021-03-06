using ITGlobal.MarkDocs.Source;
using Markdig.Renderers;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.PlantUml
{
    internal sealed class PlantUmlRenderable : IRenderable
    {
        private readonly MarkdownObject _block;
        private readonly GeneratedFileAsset _asset;
        private readonly string _url;

        public PlantUmlRenderable(MarkdownObject block, GeneratedFileAsset asset, string url)
        {
            _block = block;
            _asset = asset;
            _url = url;
        }

        public void Render(IPageRenderContext ctx, HtmlRenderer renderer)
        {
            ctx.Store(_asset);

            renderer.Write("<img src=\"");
            renderer.WriteEscapeUrl(_url);
            renderer.Write("\"");
            renderer.WriteAttributes(_block);
            renderer.Write(" alt=\"\"");
            renderer.Write(" />");
        }
    }
}