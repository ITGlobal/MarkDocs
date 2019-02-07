using System.Collections.Immutable;
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
        ///     List of compilation warnings and errors
        /// </summary>
        [NotNull]
        ImmutableDictionary<string, ImmutableArray<ICompilationReportMessage>> Messages { get; }
    }
}