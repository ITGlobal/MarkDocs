using System;
using System.IO;
using System.Net.Http;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.Mathematics
{
    /// <summary>
    ///     A MathML/Tex/LaTex renderer that uses http://latex.codecogs.com
    /// </summary>
    internal sealed class CodecogsMathRenderer : IMathRenderer
    {
        private sealed class GeneratedAsset : IGeneratedAssetContent
        {
            private readonly string _url;
            private readonly string _source;
            private readonly int? _lineNumber;

            public GeneratedAsset(string url, string source, int? lineNumber)
            {
                _url = url;
                _source = source;
                _lineNumber = lineNumber;
            }

            public string ContentType => "image/png";

            public string FormatFileName(string name) => $"math.{name}.png";

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
                    MarkdownRenderingContext.RenderContext?.Error($"Failed to render math markup. {e.Message}", _lineNumber);
                }
            }
        }

        public const string DefaultUrl = "http://latex.codecogs.com/png.download";

        private readonly string _url;

        public CodecogsMathRenderer(string url)
        {
            _url = url;
        }

        public IGeneratedAssetContent Render(string sourceCode, int? lineNumber)
        {
            return new GeneratedAsset(_url, sourceCode, lineNumber);
        }
    }
}