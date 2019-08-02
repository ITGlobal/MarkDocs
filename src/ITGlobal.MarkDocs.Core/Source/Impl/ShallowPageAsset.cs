using ITGlobal.MarkDocs.Format;
using System;
using System.IO;

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

        public PageAsset ReadAsset(IShallowPageAssetReader worker)
        {
            try
            {
                var ctx = new PageReadContext(worker, this);

                var assetMetadata = worker.GetMetadata(AbsolutePath, ctx.IsIndexPage);
                var (content, pageMetadata) = worker.Format.Read(ctx, AbsolutePath);
                var metadata = assetMetadata.MergeWith(pageMetadata);

                if (string.IsNullOrEmpty(metadata.Title))
                {
                    worker.Report.Warning(RelativePath, "No page title found");
                    metadata = metadata.WithTitle(
                        Path.GetFileNameWithoutExtension(Path.GetDirectoryName(AbsolutePath))
                    );
                }

                var asset = CreateAsset(worker, content, metadata);
                return asset;
            }
            catch (Exception e)
            {
                worker.Log.Error(e, $"Failed to read asset \"{AbsolutePath}\": {e.Message}");
                return null;
            }
        }

        public abstract void ForEach(Action<ShallowPageAsset> action);

        protected abstract PageAsset CreateAsset(IShallowPageAssetReader worker, IPageContent content, PageMetadata metadata);
    }
}