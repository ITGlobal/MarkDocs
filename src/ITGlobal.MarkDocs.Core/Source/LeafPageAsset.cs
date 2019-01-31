using ITGlobal.MarkDocs.Format;

namespace ITGlobal.MarkDocs.Source
{
    public sealed class LeafPageAsset : PageAsset
    {
        public LeafPageAsset(
            string id,
            string relativePath,
            string absolutePath,
            string contentHash,
            IParsedPage content,
            PageMetadata metadata)
            : base(id, relativePath, absolutePath, contentHash, content, metadata)
        { }
    }
}