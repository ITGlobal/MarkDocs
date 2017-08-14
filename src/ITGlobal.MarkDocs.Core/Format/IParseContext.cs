using System;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///    A context for <see cref="IFormat.ParsePage"/>
    /// </summary>
    [PublicAPI]
    public interface IParseContext
    {
        /// <summary>
        ///    A page reference
        /// </summary>
        [PublicAPI, NotNull]
        IPage Page { get; }
        
        /// <summary>
        ///     Add a warning to compilation report
        /// </summary>
        void Warning(string message, int? lineNumber = null, Exception exception = null);

        /// <summary>
        ///     Add an error to compilation report
        /// </summary>
        void Error( string message, int? lineNumber = null, Exception exception = null);
    }
}