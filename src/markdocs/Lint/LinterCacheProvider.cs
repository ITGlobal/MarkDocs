using System;
using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Tools.Lint
{
    public sealed class LinterCacheProvider : ICacheProvider
    {
        public CacheDocumentationModel[] Load() => Array.Empty<CacheDocumentationModel>();

        public ICacheUpdateTransaction BeginTransaction(
            ISourceTree sourceTree, 
            ISourceInfo sourceInfo,
            CompilationEventListener listener,
            bool forceCacheClear = false) => new LinterCacheUpdateTransaction(listener);

        public void Drop(string documentationId) { }
    }
}