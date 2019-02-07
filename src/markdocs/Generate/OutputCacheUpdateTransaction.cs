using System;
using System.IO;
using System.Text;
using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Cache.Model;
using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Tools.Generate
{
    public sealed class OutputCacheUpdateTransaction : ICacheUpdateTransaction
    {
        private readonly string _directory;
        private readonly ITemplate _template;
        
        public OutputCacheUpdateTransaction(string directory, ITemplate template)
        {
            _directory = directory;
            _template = template;

            if (Directory.Exists(_directory))
            {
                Directory.Delete(_directory, true);
            }

            Directory.CreateDirectory(_directory);

            _template.Initialize(this);
        }

        public void Dispose()
        { }

        void ICacheUpdateTransaction.Store(DocumentationModel model) { }

        void ICacheUpdateTransaction.Store(PageAsset asset, Action<Stream> write) => Store(asset, write);
        void ICacheUpdateTransaction.Store(PagePreviewAsset asset, Action<Stream> write) => Store(asset, write);
        void ICacheUpdateTransaction.Store(PhysicalFileAsset asset, Action<Stream> write) => Store(asset, write);
        void ICacheUpdateTransaction.Store(GeneratedFileAsset asset, Action<Stream> write) => Store(asset, write);

        Stream ICacheUpdateTransaction.Read(PageAsset asset) => Read(asset);
        Stream ICacheUpdateTransaction.Read(PagePreviewAsset asset) => Read(asset);
        Stream ICacheUpdateTransaction.Read(PhysicalFileAsset asset) => Read(asset);
        Stream ICacheUpdateTransaction.Read(GeneratedFileAsset asset) => Read(asset);

        public ICacheReader Commit()
        {
            return new OutputCacheReader(this);
        }

        private void Store(Asset asset, Action<Stream> write)
        {
            var filename = GetFilePath(asset);

            var directory = Path.GetDirectoryName(filename);
            Directory.CreateDirectory(directory);
            using (var stream = File.OpenWrite(filename))
            {
                write(stream);
            }
        }

        private Stream Read(Asset asset)
        {
            return File.OpenRead(GetFilePath(asset));
        }

        public string GetFilePath(IResourceId asset)
        {
            var filename = OutputCache.GetResourcePath(asset);
            if (filename.StartsWith("/"))
            {
                filename = filename.Substring(1);
            }
            filename = Path.Combine(_directory, filename);
            return filename;
        }
    }
}