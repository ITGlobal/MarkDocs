using ITGlobal.MarkDocs.Format;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Source
{
    /// <summary>
    ///     Page asset
    /// </summary>
    [PublicAPI]
    public abstract class PageAsset : Asset
    {
        /// <summary>
        ///     .ctor
        /// </summary>
        internal PageAsset(
            string id,
            string relativePath,
            string absolutePath,
            string contentHash,
            IPageContent content,
            PageMetadata metadata)
            : base(id)
        {
            Content = content;
            Metadata = metadata;
            RelativePath = relativePath;
            AbsolutePath = absolutePath;
            ContentHash = contentHash;
        }

        /// <summary>
        ///     A page asset content
        /// </summary>
        [NotNull]
        public IPageContent Content { get; }

        /// <summary>
        ///     Page metadata
        /// </summary>
        [NotNull]
        public PageMetadata Metadata { get; }

        /// <summary>
        ///     File path (relative to source tree root)
        /// </summary>
        [NotNull]
        public string RelativePath { get; }

        /// <summary>
        ///     File absolute path
        /// </summary>
        [NotNull]
        public string AbsolutePath { get; }

        /// <summary>
        ///     File content hash
        /// </summary>
        [NotNull]
        public string ContentHash { get; }

        /// <inheritdoc />
        public override ResourceType Type => ResourceType.Page;
    }
}