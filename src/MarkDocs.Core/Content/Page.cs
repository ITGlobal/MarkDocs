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
    internal sealed class Page : IPage, ICacheItem, ICacheItemContent
    {
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
            NormalizeId(ref _cacheItemId);
            _cacheItemId = Path.ChangeExtension(_node.RelativeFileName, ".html");
        }

        #endregion

        #region internal properties

        /// <summary>
        ///     Page file name
        /// </summary>
        internal string FileName => _node.FileName;

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
        ///     Page title
        /// </summary>
        public string Title => _node.Title;

        /// <summary>
        ///     Page tree node that refers to this page
        /// </summary>
        public IPageTreeNode PageTreeNode => _node;

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
                _documentation.ForceRefresh();
                stream = _cache.Read(this);
            }

            return stream;
        }

        #endregion

        #region ICacheItem

        /// <summary>
        ///     Cache item ID
        /// </summary>
        string ICacheItem.Id => _cacheItemId;

        /// <summary>
        ///     A reference to a documentation
        /// </summary>
        IDocumentation ICacheItem.Documentation => _documentation;

        /// <summary>
        ///     Cache item type
        /// </summary>
        CacheItemType ICacheItem.Type => CacheItemType.Page;

        #endregion

        #region ICacheItemContent

        /// <summary>
        ///     Gets a content
        /// </summary>
        Stream ICacheItemContent.GetContent()
        {
            try
            {
                var str = _format.RenderFile(this, FileName);
                var bytes = Encoding.UTF8.GetBytes(str);
                return new MemoryStream(bytes);
            }
            catch (Exception e)
            {
                ((MarkDocService)_documentation.Service).Log.LogError(0, e, "Failed to render page {0}!{1}", _documentation.Id, Id);
                return new MemoryStream();
            }
        }

        #endregion

        #region methods

        /// <summary>
        ///     Compiles page into cache
        /// </summary>
        internal void Compile(ICacheUpdateOperation operation)
        {
            operation.Write(this, this);
        }

        /// <summary>
        ///     Normalizes page ID
        /// </summary>
        public static void NormalizeId(ref string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return;
            }

            id = Path.Combine(Path.GetDirectoryName(id), Path.GetFileNameWithoutExtension(id));
            id = id.Replace(Path.DirectorySeparatorChar, '/');
            id = id.Replace(Path.AltDirectorySeparatorChar, '/');
            id = id.ToLowerInvariant();
        }

        #endregion
    }
}