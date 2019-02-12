using JetBrains.Annotations;
using System.Collections.Generic;

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
        [NotNull]
        IPage Page { get; }

        /// <summary>
        ///     Relative name of source file
        /// </summary>
        [NotNull]
        string SourceFileName { get; }

        /// <summary>
        ///     List of errors and warnings
        /// </summary>
        [NotNull]
        IReadOnlyList<ICompilationReportMessage> Messages { get; }
    }
}