using System.Collections.Generic;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     Provides errors and warning for a single page
    /// </summary>
    [PublicAPI]
    public interface IPageCompilationReport
    {
        /// <summary>
        ///     A page
        /// </summary>
        [PublicAPI, NotNull]
        IPage Page { get; }

        /// <summary>
        ///     Relative name of source file
        /// </summary>
        [PublicAPI, NotNull]
        string SourceFileName { get; }

        /// <summary>
        ///     List of errors and warnings
        /// </summary>
        [PublicAPI, NotNull]
        IReadOnlyList<ICompilationReportMessage> Messages { get; }
    }
}