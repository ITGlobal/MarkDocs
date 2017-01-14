using System.Collections.Generic;
using ITGlobal.MarkDocs.Format;

namespace ITGlobal.MarkDocs.Content
{
    /// <summary>
    ///     Provides page metdata from page content
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
        /// <param name="filename">
        ///     Page file name
        /// </param>
        /// <param name="consumedFiles">
        ///     Consumed content files
        /// </param>
        /// <returns>
        ///     Page metadata if available, null otherwise
        /// </returns>
        public Metadata GetMetadata(string filename, HashSet<string> consumedFiles) => _format.ParseProperties(filename);

        /// <inheritdoc />
        public void Dispose() { }
    }
}