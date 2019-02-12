using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Source
{
    /// <summary>
    ///     Page preview asset
    /// </summary>
    [PublicAPI]
    public sealed class PagePreviewAsset : Asset
    {
        /// <summary>
        ///     .ctor
        /// </summary>
        public PagePreviewAsset(string id, string contentHash)
            : base(id)
        {
            ContentHash = contentHash;
        }

        /// <summary>
        ///     File content hash
        /// </summary>
        [NotNull]
        public string ContentHash { get; }

        /// <inheritdoc />
        public override ResourceType Type => ResourceType.PagePreview;
    }
}