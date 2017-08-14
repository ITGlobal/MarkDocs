using System;

namespace ITGlobal.MarkDocs.Content
{
    internal interface IPageCompilationReportBuilder
    {
        void Warning(string message, int? lineNumber = null, Exception exception = null);
        void Error(string message, int? lineNumber = null, Exception exception = null);
    }
}