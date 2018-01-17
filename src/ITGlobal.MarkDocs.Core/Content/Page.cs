using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Format;
using System.Diagnostics;

namespace ITGlobal.MarkDocs.Content
{
    /// <summary>
    ///     A documentation page
    /// </summary>
    [DebuggerDisplay("{Id}")]
    internal sealed class Page : IPage
    {
        #region fields

        private readonly Documentation _documentation;
        private readonly IFormat _format;
        private readonly ICache _cache;
        private readonly PageTreeNode _node;
        private readonly string _cacheItemId;
        private readonly Dictionary<string, string> _anchors = new Dictionary<string, string>();

        private IParsedPage _parsedPage;
        private IResource _previewResource;

        #endregion

        #region .ctor

        /// <summary>
        ///     .ctor
        /// </summary>
        public Page(Documentation documentation, IFormat format, ICache cache, PageTreeNode node)
        {
            _documentation = documentation;
            _format = format;
            _cache = cache;
            _node = node;

            _previewResource = this;

            _cacheItemId = _node.RelativeFilePath;
            ResourceId.Normalize(ref _cacheItemId);
            _cacheItemId = Path.ChangeExtension(_node.RelativeFilePath, ".html");
        }

        #endregion

        #region internal properties

        /// <summary>
        ///     Page file name relative to content root directory
        /// </summary>
        internal string RelativeFilePath => _node.RelativeFilePath;

        #endregion

        #region IPage

        /// <summary>
        ///     A reference to a documentation
        /// </summary>
        public IDocumentation Documentation => _documentation;

        /// <summary>
        ///     Page ID
        /// </summary>
        public string Id => _node.Id;

        /// <summary>
        ///     Page file name
        /// </summary>
        public string FileName => _node.FileName;

        /// <summary>
        ///     Page title
        /// </summary>
        public string Title => _node.Title;

        /// <summary>
        ///     Page description
        /// </summary>
        public string Description => _node.Metadata.Description;

        /// <summary>
        ///     Page tree node that refers to this page
        /// </summary>
        public IPageTreeNode PageTreeNode => _node;

        /// <summary>
        ///     Resource type
        /// </summary>
        public ResourceType Type => ResourceType.Page;

        /// <summary>
        ///     Page metadata
        /// </summary>
        public Metadata Metadata => _node.Metadata;

        /// <summary>
        ///     true if page has a preview
        /// </summary>
        public bool HasPreview { get; private set; }

        /// <summary>
        ///     Page anchors (with names)
        /// </summary>
        public IReadOnlyDictionary<string, string> Anchors => _anchors;

        /// <summary>
        ///     Reads page source markup
        /// </summary>
        /// <returns>
        ///     Read-only stream
        /// </returns>
        public Stream ReadMarkup() => File.OpenRead(FileName);

        /// <summary>
        ///     Reads page rendered HTML
        /// </summary>
        /// <returns>
        ///     Read-only stream
        /// </returns>
        public Stream ReadHtml()
        {
            var stream = _cache.Read(this);
            if (stream == null)
            {
                throw new InvalidOperationException($"Unable to read cached item '{_documentation.Id}:{Id}'");
            }

            return stream;
        }

        /// <summary>
        ///     Reads page preview (HTML)
        /// </summary>
        /// <returns>
        ///     Read-only stream
        /// </returns>
        public Stream ReadPreviewHtml()
        {
            var stream = _cache.Read(_previewResource);
            if (stream == null)
            {
                throw new InvalidOperationException($"Unable to read cached item '{_documentation.Id}:{_previewResource}'");
            }

            return stream;
        }

        #endregion

        #region methods

        /// <summary>
        ///     Parses HTML page after compilation
        /// </summary>
        internal void Parse(IPageCompilationReportBuilder report)
        {
            string markup;
            try
            {
                markup = File.ReadAllText(FileName, _format.SourceEncoding);
            }
            catch (Exception e)
            {
                report.Error($"Unable to read source file. {e.Message}", null, e);
                return;
            }

            _parsedPage = _format.ParsePage(new ParseContext(this, report), markup);

            foreach (var pair in _parsedPage.Anchors)
            {
                _anchors[pair.Key] = pair.Value;
            }

            HasPreview = _parsedPage.HasPreview;
        }

        /// <summary>
        ///     Renders page into HTML
        /// </summary>
        internal void Render(ICacheUpdateOperation operation, IPageCompilationReportBuilder report, Action callback)
        {
            if (_parsedPage == null)
            {
                operation.Write(this, new EmptyResourceContent(), callback);
                return;
            }

            operation.Write(this, new ResourceContent(this, _parsedPage, report), callback);
        }

        /// <summary>
        ///     Renders page preview into HTML
        /// </summary>
        internal void RenderPreview(ICacheUpdateOperation operation, IPageCompilationReportBuilder report, Action callback)
        {
            if (_parsedPage == null || !_parsedPage.HasPreview)
            {
                return;
            }

            _previewResource = new PreviewPageResource(this);

            operation.Write(_previewResource, new PreviewResourceContent(this, _parsedPage, report), callback);

        }

        /// <summary>
        ///     Removes parsed page from memory
        /// </summary>
        internal void ReleaseParsedPage()
        {
            _parsedPage = null;
        }

        #endregion

        #region ParseContext

        private sealed class ParseContext : IParseContext
        {
            private readonly Page _page;
            private readonly IPageCompilationReportBuilder _report;

            public ParseContext(Page page, IPageCompilationReportBuilder report)
            {
                _page = page;
                _report = report;
            }

            /// <summary>
            ///    A page reference
            /// </summary>
            public IPage Page => _page;

            /// <summary>
            ///     Add a warning to compilation report
            /// </summary>
            public void Warning(string message, int? lineNumber = null, Exception exception = null)
                => _report.Warning(message, lineNumber, exception);

            /// <summary>
            ///     Add an error to compilation report
            /// </summary>
            public void Error(string message, int? lineNumber = null, Exception exception = null)
                => _report.Error(message, lineNumber, exception);
        }

        #endregion

        #region ResourceContent

        private sealed class ResourceContent : IResourceContent, IRenderContext
        {
            private readonly Page _page;
            private readonly IParsedPage _parsedPage;
            private readonly IPageCompilationReportBuilder _report;

            public ResourceContent(Page page, IParsedPage parsedPage, IPageCompilationReportBuilder report)
            {
                _page = page;
                _parsedPage = parsedPage;
                _report = report;
            }

            /// <summary>
            ///     Gets a content
            /// </summary>
            Stream IResourceContent.GetContent()
            {
                string html;
                try
                {
                    html = _parsedPage.Render(this);
                }
                catch (Exception e)
                {
                    html = "<h1 style=\"color: red;\">Failed to render page</h1>";
                    _report.Error($"Failed to render page. {e.Message}", null, e);
                }

                var bytes = Encoding.UTF8.GetBytes(html);
                return new MemoryStream(bytes);
            }

            /// <summary>
            ///   Add a generated attachment
            /// </summary>
            public IAttachment CreateAttachment(string name, byte[] content)
                => _page._documentation.CreateAttachment(name, content);

            /// <summary>
            ///    A page reference
            /// </summary>
            IPage IRenderContext.Page => _page;

            /// <summary>
            ///     Add a warning to compilation report
            /// </summary>
            void IRenderContext.Warning(string message, int? lineNumber, Exception exception)
            {
                _report.Warning(message, lineNumber, exception);
            }

            /// <summary>
            ///     Add an error to compilation report
            /// </summary>
            void IRenderContext.Error(string message, int? lineNumber, Exception exception)
            {
                _report.Error(message, lineNumber, exception);
            }
        }

        #endregion

        #region PreviewResourceContent

        private sealed class PreviewResourceContent : IResourceContent, IRenderContext
        {
            private readonly Page _page;
            private readonly IParsedPage _parsedPage;
            private readonly IPageCompilationReportBuilder _report;

            public PreviewResourceContent(Page page, IParsedPage parsedPage, IPageCompilationReportBuilder report)
            {
                _page = page;
                _parsedPage = parsedPage;
                _report = report;
            }

            /// <summary>
            ///     Gets a content
            /// </summary>
            Stream IResourceContent.GetContent()
            {
                string html;
                try
                {
                    html = _parsedPage.RenderPreview(this);
                }
                catch (Exception e)
                {
                    html = "<h1 style=\"color: red;\">Failed to render page preview</h1>";
                    _report.Error($"Failed to render page preview. {e.Message}", null, e);
                }

                var bytes = Encoding.UTF8.GetBytes(html);
                return new MemoryStream(bytes);
            }

            /// <summary>
            ///   Add a generated attachment
            /// </summary>
            public IAttachment CreateAttachment(string name, byte[] content)
                => _page._documentation.CreateAttachment(name, content);

            /// <summary>
            ///    A page reference
            /// </summary>
            IPage IRenderContext.Page => _page;

            /// <summary>
            ///     Add a warning to compilation report
            /// </summary>
            void IRenderContext.Warning(string message, int? lineNumber, Exception exception)
            {
                _report.Warning(message, lineNumber, exception);
            }

            /// <summary>
            ///     Add an error to compilation report
            /// </summary>
            void IRenderContext.Error(string message, int? lineNumber, Exception exception)
            {
                _report.Error(message, lineNumber, exception);
            }
        }

        #endregion

        #region EmptyResourceContent

        private sealed class PreviewPageResource : IResource
        {
            private readonly Page _page;

            public PreviewPageResource(Page page)
            {
                _page = page;
                Id = $"{page.Id}__preview";
            }

            public string Id { get; }
            public IDocumentation Documentation => _page.Documentation;
            public string FileName => _page.FileName;
            public ResourceType Type => ResourceType.Page;
        }

        private sealed class EmptyResourceContent : IResourceContent
        {
            /// <summary>
            ///     Gets a content
            /// </summary>
            public Stream GetContent()
            {
                var html = "<h1 style=\"color: red;\">Unable to render page</h1>";
                var bytes = Encoding.UTF8.GetBytes(html);
                return new MemoryStream(bytes);
            }
        }

        #endregion
    }
}