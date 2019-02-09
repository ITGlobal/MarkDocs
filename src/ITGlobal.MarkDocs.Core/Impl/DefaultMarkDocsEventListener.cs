using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Impl
{
    internal sealed class DefaultMarkDocsEventListener : MarkDocsEventListener
    {
        private readonly IMarkDocsLog _log;

        public DefaultMarkDocsEventListener(IMarkDocsLog log)
        {
            _log = log;
        }

        public override void SourceChanged()
        {
            _log.Debug("Source tree list change detected");
        }

        public override void SourceChanged(ISourceTree sourceTree)
        {
            _log.Debug($"[{sourceTree.Id}]: source change detected");
        }

        public override CompilationEventListener CompilationStarted(string id)
            => new DefaultCompilationEventListener(_log, id);
    }
}
