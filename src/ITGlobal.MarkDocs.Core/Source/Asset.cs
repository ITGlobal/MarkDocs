namespace ITGlobal.MarkDocs.Source
{
    public abstract class Asset : IResourceId
    {
        /// <summary>
        ///     Default MIME type
        /// </summary>
        public const string DEFAULT_MIME_TYPE = "application/octet-stream";

        protected Asset(string id)
        {
            Id = id;
        }

        public string Id { get; }
        public abstract ResourceType Type { get; }
    }
}
