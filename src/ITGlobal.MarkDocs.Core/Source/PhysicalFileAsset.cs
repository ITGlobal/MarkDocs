using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Source
{
    /// <summary>
    ///     A physical file asset
    /// </summary>
    [PublicAPI]
    public sealed class PhysicalFileAsset : FileAsset
    {
        /// <summary>
        ///     .ctor
        /// </summary>
        public PhysicalFileAsset(
            string id,
            string relativePath,
            string absolutePath,
            string contentHash,
            string contentType)
            : base(id, contentHash, contentType)
        {
            RelativePath = relativePath;
            AbsolutePath = absolutePath;
        }

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

        /// <inheritdoc />
        public override ResourceType Type => ResourceType.File;
    }
}