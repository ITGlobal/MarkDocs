using System;
using System.IO;
using System.Net.Http;
using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.Mathematics
{
    internal sealed class CodecogsGeneratedAsset : IGeneratedAssetContent
    {
        private readonly string _url;
        private readonly string _source;
        private readonly int? _lineNumber;

        public CodecogsGeneratedAsset(string url, string source, int? lineNumber)
        {
            _url = url;
            _source = source;
            _lineNumber = lineNumber;
        }

        public string ContentType => "image/png";

        public string FormatFileName(string name) => $"math/{name}.png";

        public void Write(Stream stream)
        {
            try
            {
                var url = $"{_url}?{Uri.EscapeDataString(_source)}";

                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(30);
                    var resultStream = httpClient.GetStreamAsync(url).GetAwaiter().GetResult();
                    resultStream.CopyTo(stream);
                }
            }
            catch (HttpRequestException e)
            {
                MarkdownPageRenderContext.Current?.Error($"Failed to render math markup. {e.Message}", _lineNumber);
            }
        }
    }
}