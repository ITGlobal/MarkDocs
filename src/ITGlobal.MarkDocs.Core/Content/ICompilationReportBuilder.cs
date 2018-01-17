using System;

namespace ITGlobal.MarkDocs.Content
{
    internal interface ICompilationReportBuilder
    {
        IPageCompilationReportBuilder ForFile(string path);
        IPageCompilationReportBuilder ForPage(IPage page);

        void Warning(string message, Exception exception = null);
        void Error(string message, Exception exception = null);

        ICompilationReport Build();
        void MergeWith(ICompilationReport report);
    }
}