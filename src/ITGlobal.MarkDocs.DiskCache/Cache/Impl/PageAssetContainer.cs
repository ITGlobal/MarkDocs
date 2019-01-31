using System.IO;
using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Cache.Impl
{
    internal sealed class PageAssetContainer : CachedAssetContainer<PageAsset>
    {
        public PageAssetContainer(IAssetStoreContext ctx)
            : base(ctx)
        { }

        protected override DiskCacheIndex.Dictionary SelectDictionary(DiskCacheIndex index)
            => index.Pages;

        protected override string GetHashCode(PageAsset asset)
            => asset.ContentHash;

        protected override string GetFileName(PageAsset asset)
            => Path.Combine("pages", asset.ContentHash.Substring(0, 4), $"{asset.ContentHash.Substring(4)}.html");
    }
}