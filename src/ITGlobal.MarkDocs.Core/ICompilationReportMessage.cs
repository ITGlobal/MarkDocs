using JetBrains.Annotations;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     Compilation message
    /// </summary>
    [PublicAPI]
    public interface ICompilationReportMessage
    {
        /// <summary>
        ///     Severity
        /// </summary>
        CompilationReportMessageType Type { get; }

        /// <summary>
        ///     Line number in source file
        /// </summary>
        int? LineNumber { get; }
        
        /// <summary>
        ///     Message text
        /// </summary>
        [NotNull]
        string Message { get; }
    }
}