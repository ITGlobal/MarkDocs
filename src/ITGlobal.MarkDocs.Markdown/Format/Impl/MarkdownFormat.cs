using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ITGlobal.MarkDocs.Format.Impl.Extensions.Cut;
using ITGlobal.MarkDocs.Format.Impl.Metadata;
using ITGlobal.MarkDocs.Source;
using JetBrains.Annotations;
using Markdig;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl
{
    /// <summary>
    ///     Markdown format
    /// </summary>
    internal sealed class MarkdownFormat : IFormat
    {
        #region fields

        private readonly IMarkDocsLog _log;
        private readonly IMetadataExtractor _metadataExtractor;
        private readonly IResourceUrlResolver _resourceUrlResolver;
        private readonly IMarkDocsEventCallback _callback;
        private readonly MarkdownPipeline _pipeline;

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
            [NotNull] MarkdownPipelineFactory pipelineFactory,
            [NotNull] IMarkDocsLog log,
            [NotNull] IMetadataExtractor metadataExtractor,
            [NotNull] IResourceUrlResolver resourceUrlResolver,
            [NotNull] IMarkDocsEventCallback callback)
        {
            _log = log;
            _metadataExtractor = metadataExtractor;
            _resourceUrlResolver = resourceUrlResolver;
            _callback = callback;

            _pipeline = pipelineFactory.Create();
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
        ///     Parses content of file <paramref name="filename"/>
        /// </summary>
        public (IParsedPage, PageMetadata) Read(IReadPageContext ctx, string filename)
        {
            var markup = File.ReadAllText(filename, Encoding.UTF8);
            var ast = Markdig.Markdown.Parse(markup, _pipeline);
            var properties = _metadataExtractor.Extract(ctx, ast);
            var anchors = PageAnchorReader.Read(ast);

            var previewAst = GetPreviewAst(ast, markup);
            var page = new MarkdownPage(ast, previewAst, _pipeline, _resourceUrlResolver, anchors);

            return (page, properties);
        }

        #endregion

        #region helpers

        private MarkdownDocument GetPreviewAst(MarkdownDocument ast, string markup)
        {
            var (block, index) = ast
                .Select((b, i) => (block: b, index: i))
                .FirstOrDefault(_ => _.block is CutBlock);
            if (block == null)
            {
                return null;
            }

            var previewAst = Markdig.Markdown.Parse(markup, _pipeline);
            while (previewAst.Count > index)
            {
                previewAst.RemoveAt(previewAst.Count - 1);
            }

            return previewAst;
        }

        #endregion
    }
}