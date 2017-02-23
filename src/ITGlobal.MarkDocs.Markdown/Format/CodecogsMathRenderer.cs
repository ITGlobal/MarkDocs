using System;
using System.Net.Http;
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
        public ImageData Render(string sourceCode)
        {
            var url = $"{_url}?{Uri.EscapeDataString(sourceCode)}";

            using (var httpClient = new HttpClient())
            {
                var bytes = httpClient.GetByteArrayAsync(url).Result;
                return new ImageData(bytes);
            }
        }
    }
}