using System;
using System.IO;

namespace ITGlobal.MarkDocs.Source.Impl
{
    internal sealed class StaticDirectorySourceInfo : ISourceInfo
    {
        public StaticDirectorySourceInfo(string path)
        {
            SourceUrl = "file:///" + path;
            LastChangeTime = Directory.GetLastWriteTime(path);
            LastChangeId = Guid.NewGuid().ToString("N");
        }

        public string SourceUrl { get; }
        public string Version => "";
        public DateTime? LastChangeTime { get; }
        public string LastChangeId { get; }
        public string LastChangeDescription => null;
        public string LastChangeAuthor => null;
    }
}