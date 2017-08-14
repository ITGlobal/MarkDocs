using JetBrains.Annotations;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     Compilation message severity
    /// </summary>
    [PublicAPI]
    public enum CompilationReportMessageType
    {
        /// <summary>
        ///     Warning
        /// </summary>
        Warning,

        /// <summary>
        ///     Error
        /// </summary>
        Error
    }
}