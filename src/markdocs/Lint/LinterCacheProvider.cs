using System;
using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Tools.Lint
{
    public sealed class LinterCacheProvider : ICacheProvider
    {
        public CacheDocumentationModel[] Load() => Array.Empty<CacheDocumentationModel>();

        public ICacheUpdateTransaction BeginTransaction(ISourceTree sourceTree, ISourceInfo sourceInfo,
            bool forceCacheClear = false) => new LinterCacheUpdateTransaction();

        public void Drop(string documentationId) { }
    }
}