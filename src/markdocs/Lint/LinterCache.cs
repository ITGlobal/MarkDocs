using System;
using System.Collections.Generic;
using System.IO;
using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Storage;

namespace ITGlobal.MarkDocs.Tools.Lint
{
    public sealed class LinterCache : ICache
    {
        public CacheVerifyResult Verify(IReadOnlyList<IContentDirectory> contentDirectories)
        {
            return CacheVerifyResult.OutOfDate;
        }

        public ICacheUpdateOperation_OLD BeginUpdate() => new LinterCacheUpdateOperation();

        public Stream Read(IResource item)
        {
            throw new NotSupportedException();
        }
    }
}