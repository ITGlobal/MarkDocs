using ITGlobal.MarkDocs.Extensions;

namespace ITGlobal.MarkDocs.Tools.Serve.Extensions
{
    public sealed class CompilationReportExtensionFactory : IExtensionFactory
    {
        private readonly bool _quiet;

        public CompilationReportExtensionFactory(bool quiet)
        {
            _quiet = quiet;
        }

        public IExtension Create(IMarkDocService service)
        {
            return new CompilationReportExtension(_quiet);
        }
    }
}