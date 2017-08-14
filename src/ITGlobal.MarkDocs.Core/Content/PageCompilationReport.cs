using System;
using System.Collections.Generic;

namespace ITGlobal.MarkDocs.Content
{
    internal sealed class PageCompilationReport : IPageCompilationReport, IPageCompilationReportBuilder
    {
        private readonly string _path;

        private readonly List<ICompilationReportMessage> _messages
            = new List<ICompilationReportMessage>();

        private Page _page;

        public PageCompilationReport(string path)
        {
            _path = path;
        }

        IPage IPageCompilationReport.Page => _page;

        string IPageCompilationReport.SourceFileName => _page.RelativeFilePath;

        IReadOnlyList<ICompilationReportMessage> IPageCompilationReport.Messages => _messages;

        public void SetPage(Page page)
        {
            _page = page;
        }

        public void Warning(string message, int? lineNumber = null, Exception exception = null)
        {
            _messages.Add(CompilationReportMessage.Warning(message, lineNumber, exception));
        }

        public void Error(string message, int? lineNumber = null, Exception exception = null)
        {
            _messages.Add(CompilationReportMessage.Error(message, lineNumber, exception));
        }
    }
}