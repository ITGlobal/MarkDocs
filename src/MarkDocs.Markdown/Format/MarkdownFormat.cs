using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using Markdig;
using Markdig.Extensions.MediaLinks;
using Markdig.Extensions.Tables;
using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.Extensions.Logging;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     Markdown format
    /// </summary>
    [PublicAPI]
    public sealed class MarkdownFormat : IFormat
    {
        private static readonly ThreadLocal<IPage> CurrentPageHolder = new ThreadLocal<IPage>();

        internal static IPage CurrentPage => CurrentPageHolder.Value;

        private readonly ILogger _log;
        private readonly IResourceUrlResolver _resourceUrlResolver;
        private readonly MarkdownPipeline _pipeline;

        private static readonly IMetadataExtractor[] MetadataExtractors =
        {
            new HeadingMetadataExtractor(),
            new YamlMetadataExtractor()
        };

        private static HashSet<string> _extensions = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase)
        {
            ".md",
            ".markdown"
        };

        /// <summary>
        ///     .ctor
        /// </summary>
        [PublicAPI]
        public MarkdownFormat(
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] IResourceUrlResolver resourceUrlResolver)
        {
            _log = loggerFactory.CreateLogger(typeof(MarkdownFormat));
            _resourceUrlResolver = resourceUrlResolver;
            _pipeline = CreateMarkdownPipeline(resourceUrlResolver);
        }

        /// <summary>
        ///     Get a list of page file filters
        /// </summary>
        public string[] FileFilters { get; } = { "*.md", "*.markdown" };

        /// <summary>
        ///     Get a list of index page file filters
        /// </summary>
        public string[] IndexFileNames { get; } = { "index.md", "README.md" };

        /// <summary>
        ///     Reads a page file <paramref name="filename"/> and parses it's metadata (see <see cref="Metadata"/>)
        /// </summary>
        public Metadata ParseProperties(string filename)
        {
            var properties = new Metadata();
            var document = ParseFile(filename);

            foreach (var extractor in MetadataExtractors)
            {
                extractor.TryExtract(document, properties);
            }

            return properties;
        }

        /// <summary>
        ///     Renders content of <paramref name="markup"/> into HTML
        /// </summary>
        public string Render(IPage page, string markup)
        {
            try
            {
                CurrentPageHolder.Value = page;

                var ast = Markdown.Parse(markup, _pipeline);

                RewriteLinkUrls(page, ast);

                using (var writer = new StringWriter())
                {
                    var renderer = new HtmlRenderer(writer);
                    _pipeline.Setup(renderer);
                    renderer.Render(ast);
                    writer.Flush();
                    return writer.ToString();
                }
            }
            catch (Exception)
            {
                // TODO
                throw;
            }
            finally
            {
                CurrentPageHolder.Value = null;
            }
        }

        /// <summary>
        ///     Reads a page file <paramref name="filename"/> and renders it's content into HTML
        /// </summary>
        public string RenderFile(IPage page, string filename)
        {
            try
            {
                var markdown = File.ReadAllText(filename, Encoding.UTF8);
                var html = Render(page, markdown);
                return html;
            }
            catch (Exception)
            {
                // TODO
                throw;
            }
        }

        private void RewriteLinkUrls(IPage page, MarkdownDocument ast)
        {
            foreach (var link in ast.Descendants().OfType<LinkInline>())
            {
                var url = link.Url;
                Uri uri;

                if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri))
                {
                    continue;
                }

                if (uri.IsAbsoluteUri)
                {
                    continue;
                }

                string hash = null;
                var i = url.IndexOf('#');
                if (i >= 0)
                {
                    hash = url.Substring(i);
                    url = url.Substring(0, i);
                }

                // Remove .md extension if specified
                var ext = Path.GetExtension(url);
                if (_extensions.Contains(ext))
                {
                    url = Path.ChangeExtension(url, null);
                }

                url = _resourceUrlResolver.ResolveUrl(page, url);

                if (!string.IsNullOrEmpty(hash))
                {
                    url += hash;
                }

                link.Url = url;
            }
        }

        private static MarkdownPipeline CreateMarkdownPipeline(IResourceUrlResolver resourceUrlResolver)
        {
            var builder = new MarkdownPipelineBuilder();

            builder.UseAbbreviations();
            builder.UseAutoIdentifiers();
            builder.UseCitations();
            builder.UseCustomContainers();
            builder.UseDefinitionLists();
            builder.UseDiagrams();
            builder.UseEmphasisExtras();
            builder.UseGridTables();
            builder.UseGenericAttributes();
            builder.UseFigures();
            builder.UseFooters();
            builder.UseFootnotes();

            builder.UseMathematics();
            builder.UseMediaLinks();
            builder.UsePipeTables(new PipeTableOptions
            {
                RequireHeaderSeparator = false
            });
            builder.UseListExtras();
            builder.UseTaskLists();

            builder.UseBootstrap();
            builder.UseAdvancedExtensions();
            builder.UseIcons();
            builder.UseEmojiAndSmiley();
            builder.UseSmartyPants();
            builder.UseYamlFrontMatter();
            builder.UseTableOfContents();
            builder.UseChildrenList(resourceUrlResolver);
            builder.UseCustomHeading();

            return builder.Build();
        }

        private MarkdownDocument ParseFile(string filename)
        {
            var markdown = File.ReadAllText(filename, Encoding.UTF8);
            var document = Markdown.Parse(markdown, _pipeline);
            return document;
        }
    }
}