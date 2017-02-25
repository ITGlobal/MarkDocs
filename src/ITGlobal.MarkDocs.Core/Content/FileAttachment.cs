using System.IO;
using ITGlobal.MarkDocs.Cache;

namespace ITGlobal.MarkDocs.Content
{
    /// <summary>
    ///     A file attachment
    /// </summary>
    internal sealed class FileAttachment : Attachment
    {
        #region fields

        private readonly string _path;

        #endregion

        #region .ctor

        /// <summary>
        ///     .ctor
        /// </summary>
        public FileAttachment(Documentation documentation, ICache cache, string id, string path, string contentType)
            : base(documentation, cache, id, Path.GetFileName(path), contentType)
        {
            ResourceId.Normalize(ref id);

            _path = path;
        }

        #endregion

        #region overrides of Attachment

        /// <summary>
        ///     Cache item type
        /// </summary>
        public override ResourceType Type => ResourceType.Attachment;

        /// <summary>
        ///     Gets a content
        /// </summary>
        public override Stream GetContent() => File.OpenRead(_path);

        #endregion

    }
}