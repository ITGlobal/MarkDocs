using System;
using System.Collections.Generic;
using ITGlobal.MarkDocs.Format;
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
        /// <param name="filename">
        ///     Page file name
        /// </param>
        /// <param name="consumedFiles">
        ///     Consumed content files
        /// </param>
        /// <returns>
        ///     Page metadata if available, null otherwise
        /// </returns>
        [CanBeNull]
        Metadata GetMetadata([NotNull] string filename, [NotNull] HashSet<string> consumedFiles);
    }
}