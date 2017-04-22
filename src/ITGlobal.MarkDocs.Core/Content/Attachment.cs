using System;
using System.IO;
using ITGlobal.MarkDocs.Cache;

namespace ITGlobal.MarkDocs.Content
{
    /// <summary>
    ///     An attachment base class
    /// </summary>
    internal abstract class Attachment : IAttachment, IResourceContent
    {
        #region consts

        /// <summary>
        ///     Default MIME type
        /// </summary>
        public const string DEFAULT_MIME_TYPE = "application/octet-stream";

        #endregion

        #region fields

        private readonly Documentation _documentation;
        private readonly ICache _cache;

        #endregion

        #region .ctor

        /// <summary>
        ///     .ctor
        /// </summary>
        protected Attachment(Documentation documentation, ICache cache, string id, string filename, string contentType)
        {
            ResourceId.Normalize(ref id);

            _documentation = documentation;
            _cache = cache;
            Id = id;
            FileName = filename;
            ContentType = contentType;
        }

        #endregion

        #region IAttachment

        /// <summary>
        ///     Attachment path
        /// </summary>
        public string Id { get; }

        /// <summary>
        ///     A reference to a documentation
        /// </summary>
        public IDocumentation Documentation => _documentation;

        /// <summary>
        ///     Attachment file name
        /// </summary>
        public string FileName { get; }

        /// <summary>
        ///     Attachment MIME type
        /// </summary>
        public string ContentType { get; }
        
        /// <summary>
        ///     Resource type
        /// </summary>
        public abstract ResourceType Type { get; }

        /// <summary>
        ///     Opens a read-only content stream
        /// </summary>
        public Stream Read()
        {
            var stream = _cache.Read(this);
            if (stream == null)
            {
                throw new InvalidOperationException($"Unable to read cached item '{_documentation.Id}:{Id}'");
            }

            return stream;
        }

        #endregion

        #region IResourceContent

        /// <summary>
        ///     Gets a content
        /// </summary>
        public abstract Stream GetContent();

        #endregion

        #region methods

        internal void PutIntoCache(ICacheUpdateOperation operation)
        {
            operation.Write(this, this);
        }
        
        #endregion
    }
}