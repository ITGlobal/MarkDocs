using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ITGlobal.MarkDocs.Format.Impl.Metadata;
using ITGlobal.MarkDocs.Source;
using JetBrains.Annotations;
using Markdig;

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
        private readonly MarkDocsEventListener _listener;
        private readonly MarkdownPipeline _pipeline;

        internal static readonly HashSet<string> Extensions =
            new HashSet<string>(StringComparer.CurrentCultureIgnoreCase)
            {
                ".md",
                ".markdown"
            };

        internal static readonly string[] IndexFileNames = {"index.md", "README.md"};

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
            [NotNull] MarkDocsEventListener listener)
        {
            _log = log;
            _metadataExtractor = metadataExtractor;
            _resourceUrlResolver = resourceUrlResolver;
            _listener = listener;

            _pipeline = pipelineFactory.Create();
        }

        #endregion

        #region IFormat

        /// <summary>
        ///     Get a list of page file filters
        /// </summary>
        public string[] FileFilters { get; } = {"*.md", "*.markdown"};

        /// <summary>
        ///     Get a list of index page file filters
        /// </summary>
        string[] IFormat.IndexFileNames => IndexFileNames;

        /// <summary>
        ///     Encoding for source files
        /// </summary>
        public Encoding SourceEncoding => Encoding.UTF8;

        /// <summary>
        ///     Supported extensions
        /// </summary>
        ISet<string> IFormat.Extensions => Extensions;

        /// <summary>
        ///     Parses content of file <paramref name="filename"/>
        /// </summary>
        public (IPageContent, PageMetadata) Read(IPageReadContext ctx, string filename)
        {
            var markup = File.ReadAllText(filename, Encoding.UTF8);
            var page = MarkdownPageContent.Read(ctx, _metadataExtractor, _pipeline, markup);
            return (page, page.Metadata);
        }

        #endregion

    }
}