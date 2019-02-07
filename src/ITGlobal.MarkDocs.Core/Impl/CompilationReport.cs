using ITGlobal.MarkDocs.Cache.Model;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ITGlobal.MarkDocs.Impl
{
    internal sealed class CompilationReport : ICompilationReport
    {
        public CompilationReport(CompilationReportModel model)
        {
            var dict = new Dictionary<string, List<ICompilationReportMessage>>();

            foreach (var m in model.Messages)
            {
                var location = m.Filename ?? "";
                if (!dict.TryGetValue(location, out var list))
                {
                    list = new List<ICompilationReportMessage>();
                    dict.Add(location, list);
                }

                var item = new CompilationReportMessage(m);
                list.Add(item);
            }

            Messages = dict.ToImmutableDictionary(
                _ => _.Key,
                _ => _.Value.ToImmutableArray()
            );
        }

        public ImmutableDictionary<string, ImmutableArray<ICompilationReportMessage>> Messages { get; }
    }
}