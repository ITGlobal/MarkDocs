using System;
using System.Linq;
using ITGlobal.MarkDocs.Format;

namespace ITGlobal.MarkDocs.Source.Impl
{
    internal sealed class BranchShallowPageAsset : ShallowPageAsset
    {
        public BranchShallowPageAsset(
            string id,
            string relativePath,
            string absolutePath,
            string contentHash,
            ShallowPageAsset[] subpages)
            : base(id, relativePath, absolutePath, contentHash)
        {
            Subpages = subpages;
        }

        public ShallowPageAsset[] Subpages { get; }
        
        public override void ForEach(Action<ShallowPageAsset> action)
        {
            action(this);
            foreach (var subpage in Subpages)
            {
                subpage.ForEach(action);
            }
        }

        protected override PageAsset CreateAsset(IShallowPageAssetReader worker, IPageContent content, PageMetadata metadata)
        {
            return new BranchPageAsset(
                id: Id,
                relativePath: RelativePath,
                absolutePath: AbsolutePath,
                contentHash: ContentHash,
                content: content,
                metadata: metadata,
                subpages: Subpages.Select(p => p.ReadAsset(worker)).ToArray()
            );
        }
    }
}