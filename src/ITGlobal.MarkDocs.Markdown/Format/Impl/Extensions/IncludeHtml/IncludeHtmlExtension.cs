using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using AngleSharp;
using AngleSharp.Html;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using ITGlobal.MarkDocs.Format.Impl.Extensions.PlantUml;
using ITGlobal.MarkDocs.Format.Impl.Extensions.TableOfContents;
using ITGlobal.MarkDocs.Source;
using Markdig;
using Markdig.Renderers;
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

    internal sealed class HtmlRenderable : IRenderable
    {
        private readonly GeneratedFileAsset _asset;

        public HtmlRenderable(GeneratedFileAsset asset)
        {
            _asset = asset;
        }


        public void Render(IPageRenderContext ctx, HtmlRenderer renderer)
        {
            var storedAsset = ctx.Store(_asset);

            using (var stream = storedAsset.OpenRead())
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                while (true)
                {
                    var line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }

                    renderer.WriteLine(line);
                }
            }
        }
    }
}
