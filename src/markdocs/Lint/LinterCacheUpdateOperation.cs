using System;
using ITGlobal.MarkDocs.Cache;

namespace ITGlobal.MarkDocs.Tools.Lint
{
    public sealed class LinterCacheUpdateOperation : ICacheUpdateOperation_OLD
    {
        public void Clear(IDocumentation documentation) { }

        public void Write(IResource item, IResourceContent content, Action callback)
        {
            using (content.GetContent()) { }

            if (callback != null)
            {
                callback();
            }
        }

        public void Flush() { }

        public void Commit() { }

        public void Dispose() { }
    }
}