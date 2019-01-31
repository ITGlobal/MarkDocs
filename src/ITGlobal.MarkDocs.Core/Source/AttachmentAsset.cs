namespace ITGlobal.MarkDocs.Source
{
    public sealed class AttachmentAsset : FileAsset
    {
        #region consts

        /// <summary>
        ///     Default MIME type
        /// </summary>
        public const string DEFAULT_MIME_TYPE = "application/octet-stream";

        #endregion

        public AttachmentAsset(
            string id,
            string relativePath,
            string absolutePath,
            string contentHash,
            string contentType)
            : base(id, relativePath, absolutePath, contentHash)
        {
            ContentType = contentType;
        }

        public string ContentType { get; }
    }
}