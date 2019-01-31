using System.IO;
using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Cache.Impl
{
    internal sealed class PagePreviewAssetContainer : CachedAssetContainer<PagePreviewAsset>
    {
        public PagePreviewAssetContainer(IAssetStoreContext ctx)
            : base(ctx)
        { }

        protected override DiskCacheIndex.Dictionary SelectDictionary(DiskCacheIndex index)
            => index.PagePreviews;

        protected override string GetHashCode(PagePreviewAsset asset)
            => asset.ContentHash;

        protected override string GetFileName(PagePreviewAsset asset)
            => Path.Combine("preview", asset.ContentHash.Substring(0, 4), $"{asset.ContentHash.Substring(4)}.html");
    }
}