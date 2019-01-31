using System.Collections.Generic;
using System.Linq;
using ITGlobal.MarkDocs.Cache.Model;

namespace ITGlobal.MarkDocs.Impl
{
    internal sealed class CompilationReport : ICompilationReport
    {
        public CompilationReport(IDocumentation documentation, CompilationReportModel model)
        {
            var pages = new Dictionary<string, List<ICompilationReportMessage>>();
            var common = new List<ICompilationReportMessage>();

            foreach (var m in model.Messages)
            {
                var item = new CompilationReportMessage(m);
                if (string.IsNullOrEmpty(m.Page))
                {
                    common.Add(item);
                }
                else
                {
                    if (!pages.TryGetValue(m.Page, out var list))
                    {
                        list = new List<ICompilationReportMessage>();
                        pages.Add(m.Page, list);
                    }
                    list.Add(item);
                }
            }

            Pages = pages
                .Select(p =>
                {
                    var page = documentation.GetPage(p.Key);
                    if (page == null)
                    {
                        return null;
                    }

                    return (IPageCompilationReport)(new PageCompilationReport(page, p.Value));
                })
                .Where(_ => _ != null)
                .ToList();

            Common = common;
        }

        public IReadOnlyList<IPageCompilationReport> Pages { get; }
        public IReadOnlyList<ICompilationReportMessage> Common { get; }
    }
}