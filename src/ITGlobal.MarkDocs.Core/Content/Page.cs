using System;
using System.IO;
using System.Text;
using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Format;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ITGlobal.MarkDocs.Content
{
    /// <summary>
    ///     A documentation page
    /// </summary>
    [DebuggerDisplay("{Id}")]
    internal sealed class Page : IPage
    {
        #region nested classes

        private sealed class ResourceContent : IResourceContent, IRenderContext
        {
            private readonly Page _page;
            private readonly IFormat _format;

            public ResourceContent(Page page, IFormat format)
            {
                _page = page;
                _format = format;
            }

            /// <summary>
            ///    A page reference
            /// </summary>
            public IPage Page => _page;

            /// <summary>
            ///     Gets a content
            /// </summary>
            public Stream GetContent()
            {
                string str;
                try
                {
                    var markup = File.ReadAllText(_page.FileName, _format.SourceEncoding);
                    str = _format.Render(this, markup);
                }
                catch (Exception e)
                {
                    var documentation = _page._documentation;
                    ((MarkDocService)documentation.Service).Log.LogError(0, e, "Failed to render page {0}!{1}", documentation.Id, _page.Id);
                    str = "<h1 style=\"color: red;\">Failed to render page</h1>";

                    ((MarkDocService)_page._documentation.Service).Callback.Error(_page.Documentation.Id, _page.Id, e);
                }

                var bytes = Encoding.UTF8.GetBytes(str);
                return new MemoryStream(bytes);
            }

            /// <summary>
            ///   Add a generated attachment
            /// </summary>
            public IAttachment CreateAttachment(string name, byte[] content)
                => _page._documentation.CreateAttachment(name, content);
        }

        #endregion

        #region fields

        private readonly Documentation _documentation;
        private readonly IFormat _format;
        private readonly ICache _cache;
        private readonly PageTreeNode _node;
        private readonly string _cacheItemId;

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

            _cacheItemId = _node.RelativeFileName;
            ResourceId.Normalize(ref _cacheItemId);
            _cacheItemId = Path.ChangeExtension(_node.RelativeFileName, ".html");
        }

        #endregion

        #region internal properties

        /// <summary>
        ///     Page file name relative to content root directory
        /// </summary>
        internal string RelativeFileName => _node.RelativeFileName;

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

        #endregion

        #region methods

        /// <summary>
        ///     Compiles page into cache
        /// </summary>
        internal void Compile(ICacheUpdateOperation operation)
        {
            operation.Write(this, new ResourceContent(this, _format));
        }

        #endregion
    }
}