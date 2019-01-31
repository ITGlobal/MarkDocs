using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ITGlobal.MarkDocs.Source.Impl
{
    /// <summary>
    ///     Provides page metadata from "toc.json" file
    /// </summary>
    internal sealed class TocContentMetadataProvider : IContentMetadataProvider
    {
        #region consts

        private const string TOC_FILE_NAME = "_toc.json";

        #endregion

        #region fields

        private readonly ConcurrentDictionary<string, TocFile> _entries = new ConcurrentDictionary<string, TocFile>();

        #endregion

        #region IContentMetadataProvider

        public PageMetadata GetMetadata(
            ISourceTreeRoot sourceTreeRoot,
            string filename,
            ICompilationReportBuilder report,
            HashSet<string> consumedFiles,
            bool isIndexFile)
        {
            var metadata =PageMetadata.Empty;
            if (isIndexFile)
            {
                metadata = metadata.MergeWith(GetIndexLevelMetadata(filename, report, consumedFiles));
                metadata = metadata.MergeWith(GetPlainLevelMetadata(filename, report, consumedFiles));
            }
            else
            {
                metadata = metadata.MergeWith(GetPlainLevelMetadata(filename, report, consumedFiles));
            }

            return metadata;
        }

        #endregion

        #region private methods

        private PageMetadata GetIndexLevelMetadata(string filename, ICompilationReportBuilder report, HashSet<string> consumedFiles)
        {
            var directory = Path.GetDirectoryName(filename);
            var tocFileName = Path.Combine(Path.GetDirectoryName(directory), TOC_FILE_NAME);

            var tocFile = _entries.GetOrAdd(tocFileName, path => TryReadTocFile(path, report, consumedFiles));
            if (tocFile == null)
            {
                return PageMetadata.Empty;
            }

            var properties = tocFile.TryGetMetadata(Path.GetFileName(directory));
            return properties;
        }

        private PageMetadata GetPlainLevelMetadata(string filename, ICompilationReportBuilder report, HashSet<string> consumedFiles)
        {
            var tocFileName = Path.Combine(Path.GetDirectoryName(filename), TOC_FILE_NAME);

            var tocFile = _entries.GetOrAdd(tocFileName, path => TryReadTocFile(path, report, consumedFiles));
            if (tocFile == null)
            {
                return PageMetadata.Empty;
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
                report.Error($"Unable to read {TOC_FILE_NAME}. {e.Message}");
                return null;
            }
        }

        #endregion
    }
}