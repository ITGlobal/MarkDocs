using System;
using System.Collections.Generic;
using System.Threading;
using ITGlobal.MarkDocs.Format;

namespace ITGlobal.MarkDocs.Content
{
    /// <summary>
    ///     Provides page metadata from page content
    /// </summary>
    internal sealed class ContentMetadataProvider : IMetadataProvider, IParsePropertiesContext
    {
        private readonly IFormat _format;
        private readonly ThreadLocal<IPageCompilationReportBuilder> _report = new ThreadLocal<IPageCompilationReportBuilder>();

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
            try
            {
                _report.Value = report.ForFile(filename);
                return _format.ParseProperties(this, filename);
            }
            catch (Exception e)
            {
                _report.Value.Error($"Failed to parse content metadata. {e.Message}", exception: e);
                return null;
            }
            finally
            {
                _report.Value = null;
            }
        }

        /// <inheritdoc />
        public void Dispose() { }

        void IParsePropertiesContext.Warning(string message, int? lineNumber, Exception exception)
        {
            _report.Value.Warning(message, lineNumber, exception);
        }

        void IParsePropertiesContext.Error(string message, int? lineNumber, Exception exception)
        {
            _report.Value.Error(message, lineNumber, exception);
        }
    }
}