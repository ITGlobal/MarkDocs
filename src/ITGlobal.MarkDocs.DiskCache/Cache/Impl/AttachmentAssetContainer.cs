using System.IO;
using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Cache.Impl
{
    internal sealed class AttachmentAssetContainer : CachedAssetContainer<PhysicalFileAsset>
    {
        public AttachmentAssetContainer(IAssetStoreContext ctx)
            : base(ctx)
        { }

        protected override DiskCacheIndex.Dictionary SelectDictionary(DiskCacheIndex index)
            => index.Files;

        protected override string GetHashCode(PhysicalFileAsset asset)
            => asset.ContentHash;

        protected override string GetFileName(PhysicalFileAsset asset)
            => Path.Combine("attachments", asset.ContentHash.Substring(0, 4), $"{asset.ContentHash.Substring(4)}.html");
    }
}