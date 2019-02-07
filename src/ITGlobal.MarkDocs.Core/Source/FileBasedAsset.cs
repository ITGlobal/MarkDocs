namespace ITGlobal.MarkDocs.Source
{
    public abstract class FileBasedAsset : Asset
    {
        protected FileBasedAsset(string id, string relativePath, string absolutePath, string contentHash)
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