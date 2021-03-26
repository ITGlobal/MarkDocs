using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///    A context for <see cref="IPageContent.Validate"/>
    /// </summary>
    [PublicAPI]
    public interface IPageValidateContext
    {
        /// <summary>
        ///     Gets current page resource
        /// </summary>
        [NotNull]
        IResourceId Page { get; }

        /// <summary>
        ///     Adds a warning to compilation report
        /// </summary>
        void Warning(string message, int? lineNumber = null);

        /// <summary>
        ///     Adds an error to compilation report
        /// </summary>
        void Error(string message, int? lineNumber = null);
    }
}