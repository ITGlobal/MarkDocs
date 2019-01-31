using ITGlobal.MarkDocs.Cache.Model;

namespace ITGlobal.MarkDocs.Impl
{
    internal sealed class CompilationReportMessage : ICompilationReportMessage
    {
        private readonly CompilationReportMessageModel _model;

        public CompilationReportMessage(CompilationReportMessageModel model)
        {
            _model = model;
        }

        public CompilationReportMessageType Type => _model.Type;
        public int? LineNumber => _model.LineNumber;
        public string Message => _model.Message;
    }
}