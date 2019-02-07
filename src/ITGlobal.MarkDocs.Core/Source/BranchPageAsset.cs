using ITGlobal.MarkDocs.Format;

namespace ITGlobal.MarkDocs.Source
{
    public sealed class BranchPageAsset : PageAsset
    {
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

        public PageAsset[] Subpages { get; }
    }
}