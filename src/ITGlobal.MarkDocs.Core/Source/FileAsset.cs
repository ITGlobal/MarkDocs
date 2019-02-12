using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Source
{
    /// <summary>
    ///     File asset
    /// </summary>
    [PublicAPI]
    public abstract class FileAsset : Asset
    {
        /// <summary>
        ///     .ctor
        /// </summary>
        internal FileAsset(
            string id,
            string contentHash,
            string contentType)
            : base(id)
        {
            ContentHash = contentHash;
            ContentType = contentType;
        }

        /// <summary>
        ///     File content hash
        /// </summary>
        [NotNull]
        public string ContentHash { get; }

        /// <summary>
        ///     File MIME type
        /// </summary>
        [NotNull]
        public string ContentType { get; }
    }
}