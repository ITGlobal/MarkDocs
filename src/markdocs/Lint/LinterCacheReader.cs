using System.IO;
using ITGlobal.MarkDocs.Cache;

namespace ITGlobal.MarkDocs.Tools.Lint
{
    public sealed class LinterCacheReader : ICacheReader
    {
        public Stream Read(IResource resource) => Stream.Null;
    }
}