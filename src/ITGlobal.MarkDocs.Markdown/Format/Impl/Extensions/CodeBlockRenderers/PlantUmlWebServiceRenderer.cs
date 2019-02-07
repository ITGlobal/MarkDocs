using System.IO;
using System.Net.Http;
using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.CodeBlockRenderers
{
    internal sealed class PlantUmlWebServiceRenderer : PlantUmlRenderer
    {
        private sealed class GeneratedContent : IGeneratedAssetContent
        {
            private readonly string _url;
            private readonly string _source;
            private readonly int? _lineNumber;

            public GeneratedContent(string url, string source, int? lineNumber)
            {
                _url = url;
                _source = source;
                _lineNumber = lineNumber;
            }

            public string ContentType => "image/png";

            public string FormatFileName(string name)
            {
                return $"plantuml/{name}.png";
            }

            public void Write(Stream stream)
            {
                try
                {
                    var bytes = PlantUml.Render(_url, _source);
                    stream.Write(bytes);
                }
                catch (HttpRequestException e)
                {
                    MarkdownPageRenderContext.Current?.Error($"Failed to render PlantUML markup. {e.Message}", _lineNumber);
                }
            }
        }

        public const string DefaultUrl = "http://www.plantuml.com/plantuml";

        private readonly string _url;

        public PlantUmlWebServiceRenderer(string url)
        {
            _url = url;
        }

        internal override IGeneratedAssetContent GenerateContent(string source, int? lineNumber)
        {
            return new GeneratedContent(_url, source, lineNumber);
        }
    }
}