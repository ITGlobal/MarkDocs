using System;
using System.Net.Http;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     A MathML/Tex/LaTex renderer that uses http://latex.codecogs.com
    /// </summary>
    [PublicAPI]
    public sealed class CodecogsMathRenderer : IMathRenderer
    {
        private readonly string _url;

        /// <summary>
        ///     .ctor
        /// </summary>
        [PublicAPI]
        public CodecogsMathRenderer()
            : this("http://latex.codecogs.com/png.download")
        { }

        /// <summary>
        ///     .ctor
        /// </summary>
        [PublicAPI]
        public CodecogsMathRenderer(string url)
        {
            _url = url;
        }

        /// <summary>
        ///     Render a MathML/Tex/LaTex into an image
        /// </summary>
        public ImageData Render(string sourceCode, int? lineNumber)
        {
            var bytes = RenderAsync(sourceCode, lineNumber).Result;
            return new ImageData(bytes);
        }

        private async Task<byte[]> RenderAsync(string sourceCode, int? lineNumber)
        {
            try
            {
                var url = $"{_url}?{Uri.EscapeDataString(sourceCode)}";
                
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(30);
                    var bytes = await httpClient.GetByteArrayAsync(url);
                    return bytes;
                }
            }
            catch (HttpRequestException e)
            {
                MarkdownRenderingContext.RenderContext?.Error($"Failed to render math markup. {e.Message}", lineNumber, e);
                return Array.Empty<byte>();
            }
        }
    }
}