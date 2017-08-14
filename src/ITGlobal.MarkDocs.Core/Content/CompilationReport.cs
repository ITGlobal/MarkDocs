using System.Collections.Generic;
using System.Linq;

namespace ITGlobal.MarkDocs.Content
{
    internal sealed class CompilationReport : ICompilationReport
    {
        public CompilationReport(
            IEnumerable<IPageCompilationReport> pages,
            IEnumerable<ICompilationReportMessage> common
            )
        {
            Pages = pages.Where(_ => _.Messages.Count > 0).ToList();
            Common = common.ToList();
        }

        public IReadOnlyList<IPageCompilationReport> Pages { get; }
        public IReadOnlyList<ICompilationReportMessage> Common { get; }
    }
}