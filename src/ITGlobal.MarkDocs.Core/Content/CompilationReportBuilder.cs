using System;
using System.Collections.Generic;
using System.Linq;

namespace ITGlobal.MarkDocs.Content
{
    internal sealed class CompilationReportBuilder : ICompilationReportBuilder
    {
        private readonly Dictionary<IPage, PageCompilationReport> _byPage = new Dictionary<IPage, PageCompilationReport>();
        private readonly Dictionary<string, FileCompilationReport> _byPath = new Dictionary<string, FileCompilationReport>();
        
        private readonly List<ICompilationReportMessage> _common
            = new List<ICompilationReportMessage>();

        public IPageCompilationReportBuilder ForFile(string path)
        {
            if (!_byPath.TryGetValue(path, out var report))
            {
                report = new FileCompilationReport();
                _byPath.Add(path, report);
            }

            return report;
        }

        public IPageCompilationReportBuilder ForPage(IPage page)
        {
            if (!_byPage.TryGetValue(page, out var report))
            {
                report = new PageCompilationReport((Page) page);
                _byPage.Add(page, report);
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
            var dict = _byPage.ToDictionary(_ => _.Value.Page.FileName, _=>_.Value);
            
            foreach (var p in _byPath)
            {
                if (dict.TryGetValue(p.Key, out var report))
                {
                    report.MergeWith(p.Value);
                }
            }

            return new CompilationReport(_byPage.Values, _common);
        }

        public void MergeWith(ICompilationReport report)
        {
            foreach (var pageReport in report.Pages)
            {
                var p = ForPage(pageReport.Page);
                foreach (var message in pageReport.Messages)
                {
                    switch (message.Type)
                    {
                        case CompilationReportMessageType.Warning:
                            p.Warning(message.Message, message.LineNumber, message.Exception);
                            break;
                        case CompilationReportMessageType.Error:
                            p.Error(message.Message, message.LineNumber, message.Exception);
                            break;
                    }
                }
            }

            foreach (var message in report.Common)
            {
                switch (message.Type)
                {
                    case CompilationReportMessageType.Warning:
                        Warning(message.Message, message.Exception);
                        break;
                    case CompilationReportMessageType.Error:
                        Error(message.Message, message.Exception);
                        break;
                }
            }
        }
    }
}