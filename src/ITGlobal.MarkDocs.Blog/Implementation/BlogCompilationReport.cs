using System.Collections.Generic;
using ITGlobal.MarkDocs.Content;

namespace ITGlobal.MarkDocs.Blog.Implementation
{
    internal sealed class BlogCompilationReport : ICompilationReport
    {
        private readonly List<IPageCompilationReport> _pages = new List<IPageCompilationReport>();
        private readonly List<ICompilationReportMessage> _common = new List<ICompilationReportMessage>();

        public IReadOnlyList<IPageCompilationReport> Pages => _pages;

        public IReadOnlyList<ICompilationReportMessage> Common => _common;
        
        public void AddError(string message) 
            => _common.Add(CompilationReportMessage.Error(message));

        public void AddWarning(string message) 
            => _common.Add(CompilationReportMessage.Warning(message));

        public void MergeWith(ICompilationReport report)
        {
            foreach (var message in report.Pages)
            {
                _pages.Add(message);
            }

            foreach (var message in report.Common)
            {
                _common.Add(message);
            }
        }
    }
}