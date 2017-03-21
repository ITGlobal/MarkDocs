using System;
using System.Collections.Generic;
using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Storage;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Content
{
    /// <summary>
    ///     Provides page metdata from various sources
    /// </summary>
    internal interface IMetadataProvider : IDisposable
    {
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
        [CanBeNull]
        Metadata GetMetadata(
            [NotNull] string rootDirectory,
            [NotNull] string filename, 
            [NotNull] HashSet<string> consumedFiles,
            bool isIndexFile
            );
    }
}