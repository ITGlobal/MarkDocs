namespace ITGlobal.MarkDocs.Source
{
    public interface IPageCompilationReportBuilder
    {
        void Warning(string message, int? lineNumber = null);
        void Error(string message, int? lineNumber = null);
    }
}