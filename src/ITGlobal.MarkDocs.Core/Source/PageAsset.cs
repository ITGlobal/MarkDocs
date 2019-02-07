using ITGlobal.MarkDocs.Format;

namespace ITGlobal.MarkDocs.Source
{
    public abstract class PageAsset : FileBasedAsset
    {
        protected PageAsset(
            string id,
            string relativePath,
            string absolutePath,
            string contentHash,
            IPageContent content,
            PageMetadata metadata)
            : base(id, relativePath, absolutePath, contentHash)
        {
            Content = content;
            Metadata = metadata;
        }

        public IPageContent Content { get; }
        public PageMetadata Metadata { get; }
        public override ResourceType Type => ResourceType.Page;
    }
}