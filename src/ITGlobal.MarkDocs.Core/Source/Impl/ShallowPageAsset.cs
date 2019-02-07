using System;

namespace ITGlobal.MarkDocs.Source.Impl
{
    internal abstract class ShallowPageAsset : IResourceId
    {
        protected ShallowPageAsset(
            string id,
            string relativePath,
            string absolutePath,
            string contentHash)
        {
            Id = id;
            RelativePath = relativePath;
            AbsolutePath = absolutePath;
            ContentHash = contentHash;
        }

        public string Id { get; }
        public ResourceType Type => ResourceType.Page;
        public string RelativePath { get; }
        public string AbsolutePath { get; }
        public string ContentHash { get; }

        public abstract PageAsset ReadAsset(IShallowPageAssetReader worker);
        public abstract void ForEach(Action<ShallowPageAsset> action);
    }
}