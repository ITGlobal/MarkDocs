using System.Collections.Generic;
using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Storage;

namespace ITGlobal.MarkDocs.Content
{
    /// <summary>
    ///     Provides page LastChangedBy from storage driver
    /// </summary>
    internal sealed class LastChangedByMetadataProvider : IMetadataProvider
    {
        private readonly IStorage _storage;

        /// <summary>
        ///     .ctor
        /// </summary>
        public LastChangedByMetadataProvider(IStorage storage)
        {
            _storage = storage;
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
        /// <param name="report">
        ///     Compilation report builder
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
        public Metadata GetMetadata(
            string rootDirectory,
            string filename,
            ICompilationReportBuilder report,
            HashSet<string> consumedFiles,
            bool isIndexFile)
        {
            var lastChangeAuthor = _storage.GetLastChangeAuthor(rootDirectory, filename);
            if (string.IsNullOrEmpty(lastChangeAuthor))
            {
                return null;
            }

            var properties = new Metadata { LastChangedBy = lastChangeAuthor };
            return properties;
        }

        /// <inheritdoc />
        public void Dispose() { }
    }
}