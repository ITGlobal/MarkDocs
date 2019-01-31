using System;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     Logger facade for MarkDocs
    /// </summary>
    [PublicAPI]
    public interface IMarkDocsLog
    {
        /// <summary>
        ///     Writes a debug message
        /// </summary>
        void Debug([NotNull] string message);

        /// <summary>
        ///     Writes an information message
        /// </summary>
        void Info([NotNull] string message);

        /// <summary>
        ///     Writes a warning message
        /// </summary>
        void Warning([NotNull] string message);

        /// <summary>
        ///     Writes an error message
        /// </summary>
        void Error([NotNull] string message);

        /// <summary>
        ///     Writes an error message with an exception
        /// </summary>
        void Error([NotNull] Exception e, [NotNull] string message);
    }
}