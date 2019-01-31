namespace ITGlobal.MarkDocs.Source
{
    public interface ICompilationReportBuilder
    {
        IPageCompilationReportBuilder ForFile(string path);

        void Warning(string message);
        void Error(string message);
    }
}