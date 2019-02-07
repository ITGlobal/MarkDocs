using System.IO;
using ITGlobal.MarkDocs.Cache;

namespace ITGlobal.MarkDocs.Tools.Generate
{
    public sealed class OutputCacheReader : ICacheReader
    {
        private readonly OutputCacheUpdateTransaction _transaction;

        public OutputCacheReader(OutputCacheUpdateTransaction transaction)
        {
            _transaction = transaction;
        }

        public Stream Read(IResource resource)
        {
            return File.OpenRead(_transaction.GetFilePath(resource));
        }
    }
}