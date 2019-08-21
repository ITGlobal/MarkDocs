using ITGlobal.MarkDocs.Extensions;
using ITGlobal.MarkDocs.Tools.Serve.Controllers;

namespace ITGlobal.MarkDocs.Tools.Serve.Extensions
{
    public sealed class CompilationReportExtension : IExtension
    {
        private readonly DevConnectionManager _manager;
        private readonly bool _quiet;

        public CompilationReportExtension(DevConnectionManager manager, bool quiet)
        {
            _manager = manager;
            _quiet = quiet;
        }


        public void Initialize(IMarkDocState state)
        {
            foreach (var documentation in state.List)
            {
                Program.PrintReport(documentation.CompilationReport, summary: true, quiet: _quiet);
            }
        }

        public void OnCreated(IDocumentation documentation) { }

        public void OnUpdated(IDocumentation documentation)
        {
            Program.PrintReport(documentation.CompilationReport, summary: true, quiet: _quiet);
        }

        public void OnUpdateCompleted(IDocumentation documentation)
        {
            _manager.Publish(new { type = "update", id = documentation.Id });
        }

        public void OnRemoved(IDocumentation documentation) { }
    }
}