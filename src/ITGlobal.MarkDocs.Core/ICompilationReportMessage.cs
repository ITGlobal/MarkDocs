using System;
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
        [PublicAPI]
        CompilationReportMessageType Type { get; }

        /// <summary>
        ///     A line number in a source file
        /// </summary>
        [PublicAPI, CanBeNull]
        int? LineNumber { get; }

        /// <summary>
        ///     Message text
        /// </summary>
        [PublicAPI, NotNull]
        string Message { get; }

        /// <summary>
        ///     Exception
        /// </summary>
        [PublicAPI, CanBeNull]
        Exception Exception { get; }
    }
}