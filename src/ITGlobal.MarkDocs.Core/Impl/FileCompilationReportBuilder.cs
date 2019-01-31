using System.Collections.Generic;
using ITGlobal.MarkDocs.Cache.Model;
using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Impl
{
    internal sealed class FileCompilationReportBuilder : IPageCompilationReportBuilder
    {
        private readonly List<CompilationReportMessageModel> _messages;
        private readonly string _path;

        public FileCompilationReportBuilder(List<CompilationReportMessageModel> messages, string path)
        {
            _messages = messages;
            _path = path;
        }

        public void Warning(string message, int? lineNumber = null)
        {
            _messages.Add(new CompilationReportMessageModel
            {
                Page = _path,
                LineNumber = lineNumber,
                Message = message,
                Type = CompilationReportMessageType.Warning,
            });
        }

        public void Error(string message, int? lineNumber = null)
        {
            _messages.Add(new CompilationReportMessageModel
            {
                Page = _path,
                LineNumber = lineNumber,
                Message = message,
                Type = CompilationReportMessageType.Error,
            });
        }
    }
}