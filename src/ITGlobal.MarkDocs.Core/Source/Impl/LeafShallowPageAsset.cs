using System;
using System.IO;

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

        public override PageAsset ReadAsset(IShallowPageAssetReader worker)
        {
            var metadata = worker.GetMetadata(AbsolutePath, false);
            if (string.IsNullOrEmpty(metadata.Title))
            {
                metadata = metadata.WithTitle(Path.GetFileNameWithoutExtension(AbsolutePath));
            }

            var ctx = new PageReadContext(worker, this);
            var (content, pageMetadata) = worker.Format.Read(ctx, AbsolutePath);
            metadata = metadata.MergeWith(pageMetadata);

            return new LeafPageAsset(
                id: Id,
                relativePath: RelativePath,
                absolutePath: AbsolutePath,
                contentHash: ContentHash,
                content: content,
                metadata: metadata
            );
        }

        public override void ForEach(Action<ShallowPageAsset> action)
        {
            action(this);
        }
    }
}