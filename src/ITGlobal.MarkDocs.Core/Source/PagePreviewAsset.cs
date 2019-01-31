namespace ITGlobal.MarkDocs.Source
{
    public sealed class PagePreviewAsset : Asset
    {
        public PagePreviewAsset(string id, string contentHash)
            : base(id)
        {
            ContentHash = contentHash;
        }

        public string ContentHash { get; }
    }
}