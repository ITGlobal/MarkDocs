using System.IO;
using ITGlobal.MarkDocs.Cache;

namespace ITGlobal.MarkDocs.Content
{
    /// <summary>
    ///     A attachment
    /// </summary>
    internal sealed class Attachment : IAttachment, ICacheItem, ICacheItemContent
    {
        #region consts

        /// <summary>
        ///     Default MIME type
        /// </summary>
        public const string DEFAULT_MIME_TYPE = "application/octet-stream";

        #endregion

        #region fields

        private readonly Documentation _documentation;
        private readonly string _path;
        private readonly ICache _cache;

        #endregion

        #region .ctor

        /// <summary>
        ///     .ctor
        /// </summary>
        public Attachment(Documentation documentation, ICache cache, string id, string path, string contentType)
        {
            NormalizeId(ref id);

            _documentation = documentation;
            _cache = cache;
            Id = id;
            _path = path;
            ContentType = contentType;
            FileName = System.IO.Path.GetFileName(path);
        }

        #endregion

        #region IAttachment

        /// <summary>
        ///     Attachment path
        /// </summary>
        public string Id { get; }

        /// <summary>
        ///     Attachment file name
        /// </summary>
        public string FileName { get; }

        /// <summary>
        ///     Attachment MIME type
        /// </summary>
        public string ContentType { get; }

        /// <summary>
        ///     Opens a read-only content stream
        /// </summary>
        public Stream Read()
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
        string ICacheItem.Id => Id;

        /// <summary>
        ///     A reference to a documentation
        /// </summary>
        IDocumentation ICacheItem.Documentation => _documentation;

        /// <summary>
        ///     Cache item type
        /// </summary>
        CacheItemType ICacheItem.Type => CacheItemType.Attachment;

        #endregion

        #region ICacheItemContent

        /// <summary>
        ///     Gets a content
        /// </summary>
        Stream ICacheItemContent.GetContent() => File.OpenRead(_path);

        #endregion

        #region methods

        /// <summary>
        ///     Normalizes attachment ID
        /// </summary>
        public static void NormalizeId(ref string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return;
            }

            id = Path.Combine(Path.GetDirectoryName(id), Path.GetFileName(id));
            id = id.Replace(Path.DirectorySeparatorChar, '/');
            id = id.Replace(Path.AltDirectorySeparatorChar, '/');
            id = id.ToLowerInvariant();
        }

        internal void PutIntoCache(ICacheUpdateOperation operation)
        {
            operation.Write(this, this);
        }

        #endregion
    }
}