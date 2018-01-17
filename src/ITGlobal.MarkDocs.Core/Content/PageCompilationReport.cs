using System;
using System.Collections.Generic;

namespace ITGlobal.MarkDocs.Content
{
    internal sealed class PageCompilationReport : IPageCompilationReport, IPageCompilationReportBuilder
    {
        private readonly List<ICompilationReportMessage> _messages
            = new List<ICompilationReportMessage>();

        public PageCompilationReport(Page page)
        {
            Page = page;
        }

        public IPage Page { get; set; }

        string IPageCompilationReport.SourceFileName => ((Page)Page).RelativeFilePath;

        IReadOnlyList<ICompilationReportMessage> IPageCompilationReport.Messages => _messages;

        public void Warning(string message, int? lineNumber = null, Exception exception = null)
        {
            _messages.Add(CompilationReportMessage.Warning(message, lineNumber, exception));
        }

        public void Error(string message, int? lineNumber = null, Exception exception = null)
        {
            _messages.Add(CompilationReportMessage.Error(message, lineNumber, exception));
        }

        public void MergeWith(FileCompilationReport report)
        {
            foreach (var message in report.Messages)
            {
                _messages.Add(message);
            }
        }
    }
}