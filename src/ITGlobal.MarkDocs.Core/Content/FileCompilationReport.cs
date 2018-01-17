using System;
using System.Collections.Generic;

namespace ITGlobal.MarkDocs.Content
{
    internal sealed class FileCompilationReport : IPageCompilationReportBuilder
    {
        public List<ICompilationReportMessage> Messages { get; } = new List<ICompilationReportMessage>();

        public void Warning(string message, int? lineNumber = null, Exception exception = null)
        {
            Messages.Add(CompilationReportMessage.Warning(message, lineNumber, exception));
        }

        public void Error(string message, int? lineNumber = null, Exception exception = null)
        {
            Messages.Add(CompilationReportMessage.Error(message, lineNumber, exception));
        }
    }
}