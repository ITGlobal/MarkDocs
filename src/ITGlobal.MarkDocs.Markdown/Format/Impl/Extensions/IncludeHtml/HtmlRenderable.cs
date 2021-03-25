using System.IO;
using System.Text;
using ITGlobal.MarkDocs.Source;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.IncludeHtml
{
    internal sealed class HtmlRenderable : IRenderable
    {
        private readonly GeneratedFileAsset _asset;

        public HtmlRenderable(GeneratedFileAsset asset)
        {
            _asset = asset;
        }

        public void Render(IPageRenderContext ctx, HtmlRenderer renderer)
        {
            var storedAsset = ctx.Store(_asset);

            using (var stream = storedAsset.OpenRead())
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                while (true)
                {
                    var line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }

                    renderer.WriteLine(line);
                }
            }
        }
    }
}