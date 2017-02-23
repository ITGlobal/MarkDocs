using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ITGlobal.MarkDocs.Format.Mathematics;
using JetBrains.Annotations;
using Markdig;
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
        #region fields

        private readonly ILogger _log;
        private readonly MarkdownOptions _options;
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

        #endregion

        #region .ctor

        /// <summary>
        ///     .ctor
        /// </summary>
        [PublicAPI]
        public MarkdownFormat(
            [NotNull] MarkdownOptions options,
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] IResourceUrlResolver resourceUrlResolver)
        {
            _log = loggerFactory.CreateLogger(typeof(MarkdownFormat));
            _options = options;
            _resourceUrlResolver = resourceUrlResolver;
            _pipeline = CreateMarkdownPipeline(options);
        }

        #endregion

        #region IFormat

        /// <summary>
        ///     Get a list of page file filters
        /// </summary>
        public string[] FileFilters { get; } = { "*.md", "*.markdown" };

        /// <summary>
        ///     Get a list of index page file filters
        /// </summary>
        public string[] IndexFileNames { get; } = { "index.md", "README.md" };

        /// <summary>
        ///     Encoding for source files
        /// </summary>
        public Encoding SourceEncoding => Encoding.UTF8;

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
        public string Render(IRenderContext ctx, string markup)
        {
            using (MarkdownRenderingContext.SetCurrentRenderingContext(
                _log,
                ctx,
                _options.UmlRenderer,
                _options.MathRenderer,
                _resourceUrlResolver
                ))
            {
                var ast = Markdown.Parse(markup, _pipeline);

                RewriteLinkUrls(ctx.Page, ast);

                using (var writer = new StringWriter())
                {
                    var renderer = new HtmlRenderer(writer);
                    _pipeline.Setup(renderer);
                    renderer.Render(ast);
                    writer.Flush();
                    return writer.ToString();
                }
            }
        }

        #endregion

        #region helpers

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

                url = _resourceUrlResolver.ResolveUrl(GetResourceFromUrl(page, url));

                if (!string.IsNullOrEmpty(hash))
                {
                    url += hash;
                }

                link.Url = url;
            }
        }

        private static IResource GetResourceFromUrl(IPage page, string url)
        {
            try
            {
                if (!url.StartsWith("/"))
                {
                    url = NormalizeResourcePath(page.Id, url);
                }
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException($"Href \"{url}\" is not a valid relative reference for page \"{page.Id}\"");
            }
            
            return PseudoResource.Get(page.Documentation, url);
        }

        private static string NormalizeResourcePath(string basePath, string resourceUrl)
        {
            var basePathSegments = basePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var basePathLen = basePathSegments.Length;

            var resourcePathSegments = resourceUrl.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var resourcePathLen = 0;
            for (var i = 0; i < resourcePathSegments.Length; i++)
            {
                if (resourcePathSegments[i] == "..")
                {
                    basePathLen--;
                    if (basePathLen < 0)
                    {
                        throw new InvalidOperationException("Invalid relative resource path");
                    }
                }
                else
                {
                    resourcePathLen++;
                }
            }

            var normalizedUrlSegments = new string[basePathLen + resourcePathLen];
            var index = 0;
            for (var i = 0; i < basePathLen; i++)
            {
                normalizedUrlSegments[index] = basePathSegments[i];
                index++;
            }
            for (var i = 0; i < resourcePathSegments.Length; i++)
            {
                if (resourcePathSegments[i] != "..")
                {
                    normalizedUrlSegments[index] = resourcePathSegments[i];
                    index++;
                }
            }

            var normalizedUrl = "/" + string.Join("/", normalizedUrlSegments);
            return normalizedUrl;
        }

        private static MarkdownPipeline CreateMarkdownPipeline(MarkdownOptions options)
        {
            var builder = new MarkdownPipelineBuilder();

            if (options.UseAbbreviations)
            {
                builder.UseAbbreviations();
            }
            if (options.UseAutoIdentifiers)
            {
                builder.UseAutoIdentifiers();
            }
            if (options.UseCitations)
            {
                builder.UseCitations();
            }
            if (options.UseCustomContainers)
            {
                builder.UseCustomContainers();
            }
            if (options.UseDefinitionLists)
            {
                builder.UseDefinitionLists();
            }
            if (options.UseEmphasisExtras)
            {
                builder.UseEmphasisExtras();
            }
            if (options.UseGridTables)
            {
                builder.UseGridTables();
            }
            if (options.UseGenericAttributes)
            {
                builder.UseGenericAttributes();
            }
            if (options.UseFigures)
            {
                builder.UseFigures();
            }

            if (options.UseFooters)
            {
                builder.UseFooters();
            }
            if (options.UseFootnotes)
            {
                builder.UseFootnotes();
            }
            if (options.UseMediaLinks)
            {
                builder.UseMediaLinks();
            }
            if (options.UsePipeTables)
            {
                builder.UsePipeTables(new PipeTableOptions { RequireHeaderSeparator = true });
            }
            if (options.UseListExtras)
            {
                builder.UseListExtras();
            }
            if (options.UseTaskLists)
            {
                builder.UseTaskLists();
            }
            if (options.UseBootstrap)
            {
                builder.UseBootstrap();
            }
            if (options.UseEmojiAndSmiley)
            {
                builder.UseEmojiAndSmiley();
            }
            if (options.UseSmartyPants)
            {
                builder.UseSmartyPants();
            }

            if (options.UseIcons)
            {
                builder.UseIcons();
            }

            builder.UseYamlFrontMatter();
            builder.UseTableOfContents();
            builder.UseChildrenList();
            builder.UseCustomHeading();
            builder.UseCustomCodeBlockRendering(options);

            if (options.MathRenderer != null)
            {
                builder.Extensions.Add(new MathematicsExtension());
            }

            return builder.Build();
        }

        private MarkdownDocument ParseFile(string filename)
        {
            var markdown = File.ReadAllText(filename, Encoding.UTF8);
            var document = Markdown.Parse(markdown, _pipeline);
            return document;
        }

        #endregion
    }
}