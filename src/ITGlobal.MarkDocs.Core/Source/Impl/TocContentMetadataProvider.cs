using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
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
            bool isIndexFile)
        {
            var metadata =PageMetadata.Empty;
            if (isIndexFile)
            {
                metadata = metadata.MergeWith(GetIndexLevelMetadata(filename, report));
                metadata = metadata.MergeWith(GetPlainLevelMetadata(filename, report));
            }
            else
            {
                metadata = metadata.MergeWith(GetPlainLevelMetadata(filename, report));
            }

            return metadata;
        }

        #endregion

        #region private methods

        private PageMetadata GetIndexLevelMetadata(string filename, ICompilationReportBuilder report)
        {
            var directory = Path.GetDirectoryName(filename);
            var tocFileName = Path.Combine(Path.GetDirectoryName(directory), TOC_FILE_NAME);

            var tocFile = _entries.GetOrAdd(tocFileName, path => TryReadTocFile(path, report));
            if (tocFile == null)
            {
                return PageMetadata.Empty;
            }

            var properties = tocFile.TryGetMetadata(Path.GetFileName(directory));
            return properties;
        }

        private PageMetadata GetPlainLevelMetadata(string filename, ICompilationReportBuilder report)
        {
            var tocFileName = Path.Combine(Path.GetDirectoryName(filename), TOC_FILE_NAME);

            var tocFile = _entries.GetOrAdd(tocFileName, path => TryReadTocFile(path, report));
            if (tocFile == null)
            {
                return PageMetadata.Empty;
            }

            var properties = tocFile.TryGetMetadata(Path.GetFileNameWithoutExtension(filename));
            return properties;
        }

        private static TocFile TryReadTocFile(string path, ICompilationReportBuilder report)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            try
            {
                var json = File.ReadAllText(path, Encoding.UTF8);
                var toc = JsonConvert.DeserializeObject<TocFile>(json);
                return toc;
            }
            catch (Exception e)
            {
                report.Error(path, $"Unable to read {TOC_FILE_NAME}. {e.Message}");
                return null;
            }
        }

        #endregion
    }
}