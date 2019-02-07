using System;
using System.IO;
using System.Linq;

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

        public override PageAsset ReadAsset(IShallowPageAssetReader worker)
        {
            var metadata = worker.GetMetadata(AbsolutePath, true);
            metadata = metadata ?? PageMetadata.Empty;
            if (string.IsNullOrEmpty(metadata.Title))
            {
                metadata = metadata.WithTitle(
                    Path.GetFileNameWithoutExtension(Path.GetDirectoryName(AbsolutePath))
                );
            }

            var ctx = new PageReadContext(worker, this);
            var (content, pageMetadata) = worker.Format.Read(ctx, AbsolutePath);
            metadata = metadata.MergeWith(pageMetadata);

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

        public override void ForEach(Action<ShallowPageAsset> action)
        {
            action(this);
            foreach (var subpage in Subpages)
            {
                subpage.ForEach(action);
            }
        }
    }
}