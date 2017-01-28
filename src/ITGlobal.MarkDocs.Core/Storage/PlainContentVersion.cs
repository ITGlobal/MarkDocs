using System;
using System.IO;

namespace ITGlobal.MarkDocs.Storage
{
    /// <summary>
    ///     A plain content version (source URL and last change time only)
    /// </summary>
    internal sealed class PlainContentVersion : IContentVersion
    {
        public PlainContentVersion(string path)
        {
            SourceUrl = "file:///" + path;
            LastChangeTime = Directory.GetLastWriteTime(path);
            LastChangeId = HashUtils.HashDirectory(path);
        }
        
        /// <summary>
        ///     Source URL
        /// </summary>
        public string SourceUrl { get; }

        /// <summary>
        ///     Source branch
        /// </summary>
        public string Version => null;
        
        /// <summary>
        ///     Last content change time
        /// </summary>
        public DateTime LastChangeTime { get; }

        /// <summary>
        ///     Version hash
        /// </summary>
        public string LastChangeId { get; }

        /// <summary>
        ///     Last commit message
        /// </summary>
        public string LastChangeDescription => null;

        /// <summary>
        ///     Last commit author
        /// </summary>
        public string LastChangeAuthor => null;
    }
}