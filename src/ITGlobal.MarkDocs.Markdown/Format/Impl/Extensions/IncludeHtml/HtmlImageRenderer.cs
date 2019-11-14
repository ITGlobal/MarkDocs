using System;
using System.Collections.Immutable;
using System.IO;
using AngleSharp;
using AngleSharp.Html;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Markdig.Syntax.Inlines;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.IncludeHtml
{
    internal sealed class HtmlImageRenderer : IImageRenderer
    {
        private static readonly ImmutableHashSet<string> SupportedFileExtensions =
            ImmutableHashSet.CreateRange(
                StringComparer.OrdinalIgnoreCase,
                new[] { ".html", ".htm" }
            );

        private static readonly IMarkupFormatter MarkupFormatter = new PrettyMarkupFormatter();

        public IRenderable TryCreateRenderable(IPageReadContext ctx, LinkInline obj)
        {
            var u = new Uri(obj.Url, UriKind.RelativeOrAbsolute);
            if (u.IsAbsoluteUri)
            {
                return null;
            }

            var url = obj.Url;
            url = MarkdownPageContent.NormalizeResourcePath(ctx, url);

            var ext = Path.GetExtension(obj.Url);
            if (!SupportedFileExtensions.Contains(ext))
            {
                return null;
            }

            if (!ctx.TryResolveFileResourcePath(url, out var path))
            {
                ctx.Error($"Unable to find file \"{obj.Url}\"", obj.Line);
                return null;
            }

            var filename = Path.Combine(
                Path.GetDirectoryName(url),
                Path.ChangeExtension(Path.GetFileName(url), ".html")
            );

            var markup = ReadHtml(path);
            
            ctx.CreateAttachment(markup, filename, new HtmlAssetContent(markup), out var asset, out _);
            return new HtmlRenderable(asset);
        }

        private static string ReadHtml(string path)
        {
            var parser = new HtmlParser();

            IHtmlDocument html;
            using (var file = File.OpenRead(path))
            {
                html = parser.ParseDocument(file);
            }

            string markup;
            using (var writer = new StringWriter())
            {
                html.ToHtml(writer, MarkupFormatter);
                markup = writer.ToString();
            }

            html = parser.ParseDocument(markup);
            markup = html.Body.InnerHtml;
            return markup;
        }
    }
}
