using ITGlobal.MarkDocs.Format;

namespace ITGlobal.MarkDocs.Source.Impl
{
    internal sealed class ReadPageContext : IReadPageContext
    {
        private readonly IPageCompilationReportBuilder _builder;

        public ReadPageContext(IPageCompilationReportBuilder builder)
        {
            _builder = builder;
        }

        public void Warning(string message, int? lineNumber = null)
        {
            _builder.Warning(message,lineNumber);
        }

        public void Error(string message, int? lineNumber = null)
        {
            _builder.Error(message, lineNumber);
        }
    }
}