using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Source
{
    /// <summary>
    ///     A generated on-the-fly file asset
    /// </summary>
    [PublicAPI]
    public sealed class GeneratedFileAsset : FileAsset
    {
        /// <summary>
        ///     .ctor
        /// </summary>
        public GeneratedFileAsset(string id, string relativePath, IGeneratedAssetContent content, string contentHash)
            : base(id, contentHash, content.ContentType)
        {
            RelativePath = relativePath;
            Content = content;
        }

        /// <summary>
        ///     File path (relative to source tree root).
        ///     Specified file doesn't have to exist.
        /// </summary>
        [NotNull]
        public string RelativePath { get; }

        /// <summary>
        ///     A content source for generated asset
        /// </summary>
        [NotNull]
        public IGeneratedAssetContent Content { get; }

        /// <summary>
        ///     Resource type
        /// </summary>
        public override ResourceType Type => ResourceType.GeneratedFile;
    }
}