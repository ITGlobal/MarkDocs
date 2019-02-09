using System.IO;
using System.Text;
using ITGlobal.MarkDocs.Source;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.HighlightJs
{
    internal sealed class ServerSideHighlightJsRenderable : IRenderable
    {
        private readonly GeneratedFileAsset _asset;

        public ServerSideHighlightJsRenderable(GeneratedFileAsset asset)
        {
            _asset = asset;
        }

        public void Render(IPageRenderContext ctx, HtmlRenderer renderer)
        {
            var result = ctx.Store(_asset);
            using (var stream = result.OpenRead())
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    renderer.WriteLine(line);
                }
            }
        }
    }
}