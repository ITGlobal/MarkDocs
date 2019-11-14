using System.IO;
using System.Text;
using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.IncludeHtml
{
    internal sealed class HtmlAssetContent : IGeneratedAssetContent
    {
        private readonly string _markup;

        public HtmlAssetContent(string markup)
        {
            _markup = markup;
        }

        public string ContentType => "text/html";

        public string FormatFileName(string name)
        {
            return $"html/{name}.png";
        }

        public void Write(Stream stream)
        {
            using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
            {
                writer.WriteLine(_markup);
            }
        }
    }
}