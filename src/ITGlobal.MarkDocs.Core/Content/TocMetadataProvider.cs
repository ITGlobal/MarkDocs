using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ITGlobal.MarkDocs.Format;
using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Content
{
    /// <summary>
    ///     Provides page metdata from "toc.json" file
    /// </summary>
    internal sealed class TocMetadataProvider : IMetadataProvider
    {
        #region consts

        private const string TOC_FILE_NAME = "_toc.json";

        #endregion

        #region fields

        private readonly ConcurrentDictionary<string, TocFile> _entries = new ConcurrentDictionary<string, TocFile>();

        #endregion

        #region IMetadataProvider

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
            if (isIndexFile)
            {
                var indexMetadata = GetIndexLevelMetadata(filename, report, consumedFiles);
                var plainMetadata = GetPlainLevelMetadata(filename, report, consumedFiles);

                if (indexMetadata != null)
                {
                    if (plainMetadata != null)
                    {
                        indexMetadata.CopyFrom(indexMetadata);
                    }

                    return indexMetadata;
                }

                return plainMetadata;
            }
            else
            {
                return GetPlainLevelMetadata(filename, report, consumedFiles);
            }
        }

        /// <inheritdoc />
        public void Dispose() { }


        #endregion

        #region private methods

        private Metadata GetIndexLevelMetadata(string filename, ICompilationReportBuilder report, HashSet<string> consumedFiles)
        {
            var directory = Path.GetDirectoryName(filename);
            var tocFileName = Path.Combine(Path.GetDirectoryName(directory), TOC_FILE_NAME);

            var tocFile = _entries.GetOrAdd(tocFileName, path => TryReadTocFile(path, report, consumedFiles));
            if (tocFile == null)
            {
                return null;
            }

            var properties = tocFile.TryGetMetadata(Path.GetFileName(directory));
            return properties;
        }

        private Metadata GetPlainLevelMetadata(string filename, ICompilationReportBuilder report, HashSet<string> consumedFiles)
        {
            var tocFileName = Path.Combine(Path.GetDirectoryName(filename), TOC_FILE_NAME);

            var tocFile = _entries.GetOrAdd(tocFileName, path => TryReadTocFile(path, report, consumedFiles));
            if (tocFile == null)
            {
                return null;
            }

            var properties = tocFile.TryGetMetadata(Path.GetFileNameWithoutExtension(filename));
            return properties;
        }

        private static TocFile TryReadTocFile(string path, ICompilationReportBuilder report, HashSet<string> consumedFiles)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            try
            {
                var json = File.ReadAllText(path, Encoding.UTF8);
                var toc = JsonConvert.DeserializeObject<TocFile>(json);
                consumedFiles.Add(path);
                return toc;
            }
            catch (Exception e)
            {
                report.Error($"Unable to read {TOC_FILE_NAME}. {e.Message}", e);
                return null;
            }
        }

        #endregion
    }
}