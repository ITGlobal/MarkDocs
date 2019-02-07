using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Source
{
    [PublicAPI]
    public interface ICompilationReportBuilder
    {
        void Warning([NotNull] string message);

        void Error([NotNull] string message);

        void Warning([NotNull] string path, [NotNull] string message, int? lineNumber = null);

        void Error([NotNull] string path, [NotNull] string message, int? lineNumber = null);
    }
}