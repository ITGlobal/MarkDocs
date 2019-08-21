using ITGlobal.MarkDocs.Extensions;
using ITGlobal.MarkDocs.Tools.Serve.Controllers;

namespace ITGlobal.MarkDocs.Tools.Serve.Extensions
{
    public sealed class CompilationReportExtensionFactory : IExtensionFactory
    {
        private readonly DevConnectionManager _manager;
        private readonly bool _quiet;

        public CompilationReportExtensionFactory(DevConnectionManager manager, bool quiet)
        {
            _manager = manager;
            _quiet = quiet;
        }

        public IExtension Create(IMarkDocService service)
        {
            return new CompilationReportExtension(_manager, _quiet);
        }
    }
}