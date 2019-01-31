namespace ITGlobal.MarkDocs.Source
{
    public abstract class FileAsset : Asset
    {
        protected FileAsset(string id, string relativePath, string absolutePath, string contentHash)
            : base(id)
        {
            RelativePath = relativePath;
            AbsolutePath = absolutePath;
            ContentHash = contentHash;
        }

        public string RelativePath { get; }
        public string AbsolutePath { get; }
        public string ContentHash { get; }
    }
}