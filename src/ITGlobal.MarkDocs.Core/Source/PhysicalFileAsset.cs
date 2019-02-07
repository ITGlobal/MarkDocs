namespace ITGlobal.MarkDocs.Source
{
    public sealed class PhysicalFileAsset : FileAsset
    {
        public PhysicalFileAsset(
            string id,
            string relativePath,
            string absolutePath,
            string contentHash,
            string contentType)
            : base(id, contentHash, contentType)
        {
            RelativePath = relativePath;
            AbsolutePath = absolutePath;
        }

        public string RelativePath { get; }
        public string AbsolutePath { get; }
        public override ResourceType Type => ResourceType.File;
    }
}