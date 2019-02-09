using System.IO;
using System.Net.Http;
using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.PlantUml
{
    internal sealed class PlantUmlGeneratedContent : IGeneratedAssetContent
    {
        private readonly string _url;
        private readonly string _source;
        private readonly int? _lineNumber;

        public PlantUmlGeneratedContent(string url, string source, int? lineNumber)
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
                var bytes = PlantUmlWebService.Render(_url, _source);
                stream.Write(bytes);
            }
            catch (HttpRequestException e)
            {
                MarkdownPageRenderContext.Current?.Error($"Failed to render PlantUML markup. {e.Message}", _lineNumber);
            }
        }
    }
}