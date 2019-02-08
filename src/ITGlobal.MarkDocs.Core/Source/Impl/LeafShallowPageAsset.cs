using System;
using ITGlobal.MarkDocs.Format;

namespace ITGlobal.MarkDocs.Source.Impl
{
    internal sealed class LeafShallowPageAsset : ShallowPageAsset
    {
        public LeafShallowPageAsset(
            string id,
            string relativePath,
            string absolutePath,
            string contentHash)
            : base(id, relativePath, absolutePath, contentHash)
        {
        }
        
        public override void ForEach(Action<ShallowPageAsset> action)
        {
            action(this);
        }

        protected override PageAsset CreateAsset(IShallowPageAssetReader worker, IPageContent content, PageMetadata metadata)
        {
            return new LeafPageAsset(
                id: Id,
                relativePath: RelativePath,
                absolutePath: AbsolutePath,
                contentHash: ContentHash,
                content: content,
                metadata: metadata
            );
        }
    }
}