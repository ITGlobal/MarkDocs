using System.Collections.Generic;
using ITGlobal.MarkDocs.Format;

namespace ITGlobal.MarkDocs.Content
{
    /// <summary>
    ///     Provides page metadata from page content
    /// </summary>
    internal sealed class ContentMetadataProvider : IMetadataProvider
    {
        private readonly IFormat _format;

        /// <summary>
        ///     .ctor
        /// </summary>
        public ContentMetadataProvider(IFormat format)
        {
            _format = format;
        }

        /// <summary>
        ///     Gets a metadata for a specified page
        /// </summary>
        /// <param name="rootDirectory">
        ///     Content root directory
        /// </param>
        /// <param name="filename">
        ///     Page file name
        /// </param>
        /// <param name="consumedFiles">
        ///     Consumed content files
        /// </param>
        /// <param name="isIndexFile">
        ///     true is <paramref name="filename"/> is an index page file.
        /// </param>
        /// <returns>
        ///     Page metadata if available, null otherwise
        /// </returns>
        public Metadata GetMetadata(string rootDirectory, string filename, HashSet<string> consumedFiles, bool isIndexFile)
            => _format.ParseProperties(filename);

        /// <inheritdoc />
        public void Dispose() { }
    }
}