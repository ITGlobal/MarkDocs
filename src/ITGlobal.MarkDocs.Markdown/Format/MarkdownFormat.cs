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
        private readonly IMarkDocsEventCallback _callback;
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
            [NotNull] IMarkDocsEventCallback callback,
            [NotNull] IServiceProvider serviceProvider)
        {
            _log = loggerFactory.CreateLogger(typeof(MarkdownFormat));
            _options = options;
            _resourceUrlResolver = options.ResourceUrlResolver;
            _callback = callback;

            options.SyntaxColorizer.Initialize(_log);
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
                _options,
                _resourceUrlResolver
                ))
            {
                var ast = Markdig.Markdown.Parse(markup, _pipeline);

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
                if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri))
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

                var isIndexFileLink = false;
                var filename = Path.GetFileName(url);
                foreach (var name in IndexFileNames)
                {
                    if (filename == name)
                    {
                        url = Path.GetDirectoryName(url);
                        isIndexFileLink = true;
                        break;
                    }
                }

                // Remove .md extension if specified
                if (!isIndexFileLink)
                {
                    var ext = Path.GetExtension(url);
                    if (_extensions.Contains(ext))
                    {
                        url = Path.ChangeExtension(url, null);
                    }
                }

                url = _resourceUrlResolver.ResolveUrl(GetResourceFromUrl(page, link, url), page);

                if (!string.IsNullOrEmpty(hash))
                {
                    url += hash;
                }

                link.Url = url;
            }
        }

        private IResource GetResourceFromUrl(IPage page, LinkInline link, string url)
        {
            try
            {
                if (!url.StartsWith("/") && !url.StartsWith("\\"))
                {
                    url = NormalizeResourcePath(page, url);
                }
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException($"Href \"{url}\" is not a valid relative reference for page \"{page.Id}\"");
            }

            ResourceType type;
            if (page.Documentation.GetPage(url) != null)
            {
                type = ResourceType.Page;
            }
            else if (page.Documentation.GetAttachment(url) != null)
            {
                type = ResourceType.Attachment;
            }
            else
            {
                _callback.Warning(page.Documentation.Id, page.Id, $"Invalid hyperlink: \"{url}\"", $"({link.Line}, {link.Column})");
                type = ResourceType.Attachment;
            }

            var resource = PseudoResource.Get(page.Documentation, url, GetResourceFileName(url), type);
            return resource;
        }

        private static string NormalizeResourcePath(IPage page, string resourceUrl)
        {
            var basePath = page.Id;

            var basePathSegments = basePath.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            var basePathLen = basePathSegments.Length;
            if (!page.PageTreeNode.IsIndexPage)
            {
                basePathLen--;
            }

            var resourcePathSegments = resourceUrl.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
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

        private static string GetResourceFileName(string url)
        {
            var filename = Path.GetFileName(url);

            var ext = Path.GetExtension(filename);
            if (ext == ".md" || ext == ".markdown")
            {
                filename = Path.ChangeExtension(filename, ".html");
            }

            return filename;
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
            builder.UseAdmonitions();

            if (options.MathRenderer != null)
            {
                builder.Extensions.Add(new MathematicsExtension());
            }

            builder.Extensions.Add(options.Rendering);
            
            return builder.Build();
        }

        private MarkdownDocument ParseFile(string filename)
        {
            var markdown = File.ReadAllText(filename, Encoding.UTF8);
            var document = Markdig.Markdown.Parse(markdown, _pipeline);
            return document;
        }

        #endregion
    }
}