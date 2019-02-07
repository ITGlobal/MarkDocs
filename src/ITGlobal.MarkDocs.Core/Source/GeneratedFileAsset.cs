namespace ITGlobal.MarkDocs.Source
{
    public sealed class GeneratedFileAsset : FileAsset
    {
        public GeneratedFileAsset(string id, string relativePath, IGeneratedAssetContent content, string contentHash)
            : base(id, contentHash, content.ContentType)
        {
            RelativePath = relativePath;
            Content = content;
        }

        public string RelativePath { get; }
        public IGeneratedAssetContent Content { get; }
        public override ResourceType Type => ResourceType.GeneratedFile;
    }
}