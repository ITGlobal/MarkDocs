using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///    A context for <see cref="IFormat.Read"/>
    /// </summary>
    [PublicAPI]
    public interface IReadPageContext
    {
        /// <summary>
        ///     Add a warning to compilation report
        /// </summary>
        void Warning(string message, int? lineNumber = null);

        /// <summary>
        ///     Add an error to compilation report
        /// </summary>
        void Error(string message, int? lineNumber = null);
    }
}