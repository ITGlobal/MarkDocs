using System.Collections.Generic;

namespace ITGlobal.MarkDocs.Impl
{
    internal sealed class PageCompilationReport : IPageCompilationReport
    {
        public PageCompilationReport(IPage page, IReadOnlyList<ICompilationReportMessage> messages)
        {
            Page = page;
            Messages = messages;
        }

        public IPage Page { get; }
        public string SourceFileName => Page.RelativePath;
        public IReadOnlyList<ICompilationReportMessage> Messages { get; }
    }
}