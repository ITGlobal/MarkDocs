using System.IO;
using ITGlobal.MarkDocs.Impl;

namespace ITGlobal.MarkDocs.Source.Impl
{
    internal sealed class Sha1ContentHashProvider : IContentHashProvider
    {
        public bool TryGetContentHash(string path, out string contentHash)
        {
            using (var stream = File.OpenRead(path))
            {
                if (stream == null)
                {
                    contentHash = null;
                    return false;
                }

                contentHash = HashUtil.HashStream(stream);
                return true;
            }
        }
    }
}
