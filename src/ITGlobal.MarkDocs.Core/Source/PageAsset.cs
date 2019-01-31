using ITGlobal.MarkDocs.Format;

namespace ITGlobal.MarkDocs.Source
{
    public abstract class PageAsset : FileAsset
    {
        protected PageAsset(
            string id,
            string relativePath,
            string absolutePath,
            string contentHash,
            IParsedPage content,
            PageMetadata metadata)
            : base(id, relativePath, absolutePath, contentHash)
        {
            Content = content;
            Metadata = metadata;
        }

        public IParsedPage Content { get; }
        public PageMetadata Metadata { get; }
    }
}