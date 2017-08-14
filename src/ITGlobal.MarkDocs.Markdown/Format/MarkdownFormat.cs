using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using AngleSharp.Parser.Html;
using ITGlobal.MarkDocs.Format.Mathematics;
using JetBrains.Annotations;
using Markdig;
using Markdig.Extensions.Tables;
using Markdig.Renderers;
using Markdig.Syntax;
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

        internal static readonly HashSet<string> Extensions = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase)
        {
            ".md",
            ".markdown"
        };

        internal static readonly string[] IndexFileNames = { "index.md", "README.md" };

        #endregion

        #region .ctor

        /// <summary>
        ///     .ctor
        /// </summary>
        [PublicAPI]
        public MarkdownFormat(
            [NotNull] MarkdownOptions options,
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] IMarkDocsEventCallback callback)
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
        string[] IFormat.IndexFileNames => IndexFileNames;

        /// <summary>
        ///     Encoding for source files
        /// </summary>
        public Encoding SourceEncoding => Encoding.UTF8;

        /// <summary>
        ///     Reads a page file <paramref name="filename"/> and parses it's metadata (see <see cref="Metadata"/>)
        /// </summary>
        public Metadata ParseProperties(IParsePropertiesContext ctx, string filename)
        {
            var properties = new Metadata();
            var document = ParseFile(filename);

            foreach (var extractor in MetadataExtractors)
            {
                extractor.TryExtract(ctx, document, properties);
            }

            return properties;
        }

        /// <summary>
        ///     Parses content of <paramref name="markup"/>
        /// </summary>
        public IParsedPage ParsePage(IParseContext ctx, string markup)
        {
            using (MarkdownParseContext.SetCurrentParseContext(ctx))
          //  using (MarkdownRenderingContext.SetCurrentRenderingContext(new FakeRenderContext(ctx), _options, _resourceUrlResolver))
            {
                // Parse markdown
                var ast = Markdig.Markdown.Parse(markup, _pipeline);

                // Render HTML (temporary)
                string html;
                using (var writer = new StringWriter())
                {
                    var renderer = new HtmlRenderer(writer);
                    _pipeline.Setup(renderer);
                    renderer.Render(ast);
                    writer.Flush();

                    html = writer.ToString();
                }

                // Extract anchors
                var anchors = new Dictionary<string, string>();
                ExtractAnchors(ctx, html, anchors);

                return new MarkdownPage(ast, _options, _pipeline, _resourceUrlResolver, anchors);
            }
        }

        #endregion

        #region helpers

        private static void ExtractAnchors(IParseContext ctx, string html, Dictionary<string, string> anchors)
        {
            var parser = new HtmlParser();
            var document = parser.Parse(html);
            foreach (var node in document.Body.Descendents())
            {
                switch (node)
                {
                    case IHtmlHeadingElement heading:
                        if (!string.IsNullOrEmpty(heading.Id))
                        {
                            var id = heading.Id;
                            var text = heading.Text();
                            if (!anchors.ContainsKey(id))
                            {
                                anchors.Add(id, text);
                            }
                            else
                            {
                                ctx.Warning($"Anchor \"{id}\" is defined more than once");
                            }
                        }
                        break;
                }
            }
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
            builder.UseAlerts();

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