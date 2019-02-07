using System;
using System.IO;
using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Cache.Model;
using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Tools.Lint
{
    public sealed class LinterCacheUpdateTransaction : ICacheUpdateTransaction
    {
        public void Store(DocumentationModel model) { }

        public void Store(PageAsset asset, Action<Stream> write)
        {
            write(Stream.Null);
        }

        public void Store(PagePreviewAsset asset, Action<Stream> write)
        {
            write(Stream.Null);
        }

        public void Store(PhysicalFileAsset asset, Action<Stream> write)
        {
            write(Stream.Null);
        }

        public void Store(GeneratedFileAsset asset, Action<Stream> write)
        {
            write(Stream.Null);
        }

        public Stream Read(PageAsset asset) => Stream.Null;

        public Stream Read(PagePreviewAsset asset) => Stream.Null;

        public Stream Read(PhysicalFileAsset asset) => Stream.Null;

        public Stream Read(GeneratedFileAsset asset) => Stream.Null;

        public ICacheReader Commit() => new LinterCacheReader();

        public void Dispose() { }
    }
}