using System.IO;
using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Cache.Impl
{
    internal sealed class AttachmentAssetContainer : CachedAssetContainer<AttachmentAsset>
    {
        public AttachmentAssetContainer(IAssetStoreContext ctx)
            : base(ctx)
        { }

        protected override DiskCacheIndex.Dictionary SelectDictionary(DiskCacheIndex index)
            => index.Files;

        protected override string GetHashCode(AttachmentAsset asset)
            => asset.ContentHash;

        protected override string GetFileName(AttachmentAsset asset)
            => Path.Combine("attachments", asset.ContentHash.Substring(0, 4), $"{asset.ContentHash.Substring(4)}.html");
    }
}