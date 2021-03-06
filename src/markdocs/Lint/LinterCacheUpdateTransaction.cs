using System;
using System.IO;
using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Cache.Model;
using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Tools.Lint
{
    public sealed class LinterCacheUpdateTransaction : ICacheUpdateTransaction
    {
        private readonly CompilationEventListener _listener;

        public LinterCacheUpdateTransaction(CompilationEventListener listener)
        {
            _listener = listener;
        }

        public void Store(DocumentationModel model) { }

        public void Store(PageAsset asset, Action<Stream> write)
        {
            write(Stream.Null);
            _listener.Written(asset);
        }

        public void Store(PagePreviewAsset asset, Action<Stream> write)
        {
            write(Stream.Null);
            _listener.Written(asset);
        }

        public void Store(PhysicalFileAsset asset, Action<Stream> write)
        {
            write(Stream.Null);
            _listener.Written(asset);
        }

        public void Store(GeneratedFileAsset asset, Action<Stream> write)
        {
            write(Stream.Null);
            _listener.Written(asset);
        }

        public Stream Read(PageAsset asset) => Stream.Null;

        public Stream Read(PagePreviewAsset asset) => Stream.Null;

        public Stream Read(PhysicalFileAsset asset) => Stream.Null;

        public Stream Read(GeneratedFileAsset asset) => Stream.Null;

        public ICacheReader Commit() => new LinterCacheReader();

        public void Dispose() { }
    }
}