using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Source
{
    /// <summary>
    ///     Compilation report builder
    /// </summary>
    [PublicAPI]
    public interface ICompilationReportBuilder
    {
        /// <summary>
        ///     Adds an error message
        /// </summary>
        void Error([NotNull] string message);

        /// <summary>
        ///     Adds a warning message
        /// </summary>
        void Warning([NotNull] string path, [NotNull] string message, int? lineNumber = null);

        /// <summary>
        ///     Adds an error message
        /// </summary>
        void Error([NotNull] string path, [NotNull] string message, int? lineNumber = null);
    }
}