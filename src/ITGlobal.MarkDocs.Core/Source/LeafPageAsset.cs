using ITGlobal.MarkDocs.Format;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Source
{
    /// <summary>
    ///     Leaf page asset
    /// </summary>
    [PublicAPI]
    public sealed class LeafPageAsset : PageAsset
    {
        /// <summary>
        ///     .ctor
        /// </summary>
        public LeafPageAsset(
            string id,
            string relativePath,
            string absolutePath,
            string contentHash,
            IPageContent content,
            PageMetadata metadata)
            : base(id, relativePath, absolutePath, contentHash, content, metadata)
        { }
    }
}