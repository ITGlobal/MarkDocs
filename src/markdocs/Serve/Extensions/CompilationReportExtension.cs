using ITGlobal.MarkDocs.Extensions;

namespace ITGlobal.MarkDocs.Tools.Serve.Extensions
{
    public sealed class CompilationReportExtension : IExtension
    {
        private readonly bool _quiet;

        public CompilationReportExtension(bool quiet)
        {
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

        public void OnUpdateCompleted(IDocumentation documentation) { }

        public void OnRemoved(IDocumentation documentation) { }
    }
}