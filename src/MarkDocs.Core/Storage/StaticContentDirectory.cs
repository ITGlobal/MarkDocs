namespace ITGlobal.MarkDocs.Storage
{
    /// <summary>
    ///     Static content directory
    /// </summary>
    internal sealed class StaticContentDirectory : IContentDirectory
    {
        public StaticContentDirectory(string id, string path)
        {
            Id = id;
            Path = path;
            ContentVersion = new PlainContentVersion(path);
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