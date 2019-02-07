namespace ITGlobal.MarkDocs.Source
{
    public abstract class FileAsset : Asset
    {
        protected FileAsset(
            string id,
            string contentHash,
            string contentType)
            : base(id)
        {
            ContentHash = contentHash;
            ContentType = contentType;
        }

        public string ContentHash { get; }
        public string ContentType { get; }
    }
}