using System.IO;
using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Cache.Impl
{
    internal sealed class GeneratedAttachmentAssetContainer : CachedAssetContainer<GeneratedAsset>
    {
        public GeneratedAttachmentAssetContainer(IAssetStoreContext ctx)
            : base(ctx)
        { }

        protected override DiskCacheIndex.Dictionary SelectDictionary(DiskCacheIndex index)
            => index.GeneratedFiles;

        protected override string GetHashCode(GeneratedAsset asset)
            => asset.ContentHash;

        protected override string GetFileName(GeneratedAsset asset)
            => Path.Combine("generated", asset.ContentHash.Substring(0, 4), $"{asset.ContentHash.Substring(4)}{Path.GetExtension(asset.RelativePath)}");
    }
}