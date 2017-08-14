using System;
using System.Collections.Generic;

namespace ITGlobal.MarkDocs.Content
{
    internal sealed class CompilationReportBuilder : ICompilationReportBuilder
    {
        private readonly Dictionary<Page, PageCompilationReport> _byPage
            = new Dictionary<Page, PageCompilationReport>();
        private readonly Dictionary<string, PageCompilationReport> _byPath
            = new Dictionary<string, PageCompilationReport>();
        private readonly List<IPageCompilationReport> _list
            = new List<IPageCompilationReport>();

        private readonly List<ICompilationReportMessage> _common
            = new List<ICompilationReportMessage>();
        
        public IPageCompilationReportBuilder ForFile(string path)
        {
            if (!_byPath.TryGetValue(path, out var report))
            {
                report = new PageCompilationReport(path);
                _byPath.Add(path, report);
                _list.Add(report);
            }

            return report;
        }

        public IPageCompilationReportBuilder ForPage(Page page)
        {
            if (!_byPage.TryGetValue(page, out var report))
            {
                if (!_byPath.TryGetValue(page.FileName, out report))
                {
                    report = new PageCompilationReport(page.FileName);
                }
                else
                {
                    report.SetPage(page);
                }

                _byPage.Add(page, report);
                _list.Add(report);
            }

            return report;
        }

        public void Warning(string message, Exception exception = null)
        {
            _common.Add(CompilationReportMessage.Warning(message, null, exception));
        }

        public void Error(string message, Exception exception = null)
        {
            _common.Add(CompilationReportMessage.Error(message, null, exception));
        }

        public ICompilationReport Build()
        {
            return new CompilationReport(_list, _common);
        }
    }
}