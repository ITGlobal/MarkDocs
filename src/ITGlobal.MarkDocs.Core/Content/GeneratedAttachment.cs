using System.IO;
using ITGlobal.MarkDocs.Cache;

namespace ITGlobal.MarkDocs.Content
{
    /// <summary>
    ///     A attachment generated on the fly
    /// </summary>
    internal sealed class GeneratedAttachment : Attachment
    {
        #region fields

        private readonly byte[] _content;

        #endregion

        #region .ctor

        /// <summary>
        ///     .ctor
        /// </summary>
        public GeneratedAttachment(Documentation documentation, ICache cache, string id, string filename, string contentType, byte[] content)
            : base(documentation, cache, id, filename, contentType)
        {
            _content = content;
        }

        #endregion

        #region overrides of Attachment

        /// <summary>
        ///     Cache item type
        /// </summary>
        public override CacheItemType Type => CacheItemType.Illustration;

        /// <summary>
        ///     Gets a content
        /// </summary>
        public override Stream GetContent() => new MemoryStream(_content);

        #endregion
    }
}