using System.Collections.Generic;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     Provides errors and warning for documentation
    /// </summary>
    [PublicAPI]
    public interface ICompilationReport
    {
        /// <summary>
        ///     List of compilation warnings and errors for every page
        /// </summary>
        [PublicAPI, NotNull]
        IReadOnlyList<IPageCompilationReport> Pages { get; }

        /// <summary>
        ///     List of compilation warnings and errors than are not connected to any specific page
        /// </summary>
        [PublicAPI, NotNull]
        IReadOnlyList<ICompilationReportMessage> Common { get; }
    }
}