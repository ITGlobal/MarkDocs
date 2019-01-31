using System.Collections.Generic;
using ITGlobal.MarkDocs.Cache.Model;
using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Impl
{
    internal sealed class CompilationReportBuilder : ICompilationReportBuilder
    {
        private readonly Dictionary<IPage, PageCompilationReport> _byPage = new Dictionary<IPage, PageCompilationReport>();
        private readonly Dictionary<string, FileCompilationReportBuilder> _byPath = new Dictionary<string, FileCompilationReportBuilder>();
        
        private readonly List<CompilationReportMessageModel> _messages
            = new List<CompilationReportMessageModel>();

        public IPageCompilationReportBuilder ForFile(string path)
        {
            if (!_byPath.TryGetValue(path, out var report))
            {
                report = new FileCompilationReportBuilder(_messages, path);
                _byPath.Add(path, report);
            }

            return report;
        }
        
        public void Warning(string message)
        {
            _messages.Add(new CompilationReportMessageModel
            {
                Message = message,
                Type = CompilationReportMessageType.Warning,
            });
        }

        public void Error(string message)
        {
            _messages.Add(new CompilationReportMessageModel
            {
                Message = message,
                Type = CompilationReportMessageType.Error,
            });
        }

        public CompilationReportModel Build()
        {
            return new CompilationReportModel
            {
                Messages = _messages.ToArray()
            };
        }
    }
}