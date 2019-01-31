namespace ITGlobal.MarkDocs.Source
{
    public sealed class GeneratedAsset : Asset
    {
        public GeneratedAsset(string id, string relativePath, string contentType, string contentHash)
            : base(id)
        {
            RelativePath = relativePath;
            ContentType = contentType;
            ContentHash = contentHash;
        }

        public string RelativePath { get; }
        public string ContentType { get; }
        public string ContentHash { get; }
    }
}