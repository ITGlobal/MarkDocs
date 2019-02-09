using System.IO;
using System.Text;
using ITGlobal.MarkDocs.Source;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.HighlightJs
{
    internal sealed class ServerSideHighlightJsCodeBlockRenderer : ICodeBlockRenderer
    {
        private sealed class HighlightJsCacheData : IGeneratedAssetContent
        {
            private readonly HighlightJsWorker _hljs;
            private readonly string _language;
            private readonly string _sourceCode;

            public HighlightJsCacheData(HighlightJsWorker hljs, string language, string sourceCode)
            {
                _hljs = hljs;
                _language = language;
                _sourceCode = sourceCode;
            }

            public string ContentType => "text/html";

            public string FormatFileName(string name) => $"hljs/{name}.html";

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

        public IRenderable TryCreateRenderable(IPageReadContext ctx, FencedCodeBlock block)
        {
            var language = block.Info;
            if (!_hljs.SupportedLanguages.Contains(language))
            {
                return null;
            }
            
            var markup = block.GetText();

            ctx.CreateAttachment(
                markup,
                new HighlightJsCacheData(_hljs, language, markup),
                out var asset,
                out _
            );
            return new ServerSideHighlightJsRenderable(asset);
        }
    }
}
