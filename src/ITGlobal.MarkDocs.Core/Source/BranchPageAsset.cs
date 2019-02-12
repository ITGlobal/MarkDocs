using ITGlobal.MarkDocs.Format;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Source
{
    /// <summary>
    ///     Branch page asset
    /// </summary>
    [PublicAPI]
    public sealed class BranchPageAsset : PageAsset
    {
        /// <summary>
        ///     .ctor
        /// </summary>
        public BranchPageAsset(
            string id,
            string relativePath,
            string absolutePath,
            string contentHash,
            IPageContent content,
            PageMetadata metadata,
            PageAsset[] subpages)
            : base(id, relativePath, absolutePath, contentHash, content, metadata)
        {
            Subpages = subpages;
        }

        /// <summary>
        ///     Nested pages assets
        /// </summary>
        [NotNull]
        public PageAsset[] Subpages { get; }
    }
}