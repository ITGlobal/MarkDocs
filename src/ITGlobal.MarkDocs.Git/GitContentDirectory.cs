using ITGlobal.MarkDocs;
using ITGlobal.MarkDocs.Storage;

namespace ITGlobal.MarkDocs.Git
{
    /// <summary>
    ///     Git content directory description
    /// </summary>
    internal sealed class GitContentDirectory : IContentDirectory
    {
        /// <summary>
        ///     .ctor   
        /// </summary>
        public GitContentDirectory(string id, string path, IContentVersion contentVersion)
        {
            Id = id;
            Path = path;
            ContentVersion = contentVersion;
        }

        /// <summary>
        ///     Documentation ID
        /// </summary>
        public string Id { get; }

        /// <summary>
        ///     Path to a content directory
        /// </summary>
        public string Path { get; }

        /// <summary>
        ///     Content version information
        /// </summary>
        public IContentVersion ContentVersion { get; }
    }
}