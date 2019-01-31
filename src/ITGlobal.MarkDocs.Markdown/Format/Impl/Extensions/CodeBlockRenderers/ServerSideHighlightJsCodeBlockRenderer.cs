using Markdig.Renderers;
using Markdig.Syntax;
using System.IO;
using System.Text;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.CodeBlockRenderers
{
    internal sealed class ServerSideHighlightJsCodeBlockRenderer : ICodeBlockRenderer
    {
        private sealed class HljsCacheData : IGeneratedAssetContent
        {
            private readonly HighlightJsWorker _hljs;
            private readonly string _language;
            private readonly string _sourceCode;

            public HljsCacheData(HighlightJsWorker hljs, string language, string sourceCode)
            {
                _hljs = hljs;
                _language = language;
                _sourceCode = sourceCode;
            }

            public string ContentType => "text/html";

            public string FormatFileName(string name) => $"hljs.{name}.html";

            public void Write(Stream stream)
            {
                using (var w = new StreamWriter(stream, Encoding.UTF8, 1024, leaveOpen: true))
                {
                    var html = _hljs.Render(_language, _sourceCode);
                    w.Write(html);
                    w.Flush();
                }
            }
        }

        private readonly HighlightJsWorker _hljs;

        static ServerSideHighlightJsCodeBlockRenderer()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public ServerSideHighlightJsCodeBlockRenderer(HighlightJsWorker hljs)
        {
            _hljs = hljs;
        }

        public bool CanRender(IPageRenderContext ctx, FencedCodeBlock block)
            => _hljs.SupportedLanguages.Contains(block.Info);

        public void Render(IPageRenderContext ctx, HtmlRenderer renderer, FencedCodeBlock block)
        {
            var language = block.Info;
            var markup = block.GetText();

            var result = MarkdownRenderingContext.RenderContext.CreateAttachment(
                markup,
                new HljsCacheData(_hljs, language, markup)
            );
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