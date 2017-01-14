using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Extensions
{
    /// <summary>
    ///     MarkDocs extension factory
    /// </summary>
    [PublicAPI]
    public interface IExtensionFactory
    {
        /// <summary>
        ///     Create an instance of an extension
        /// </summary>
        /// <param name="service">
        ///     MarkDocs service
        /// </param>
        /// <param name="state">
        ///     Initial documentation state
        /// </param>
        /// <returns>
        ///     MarkDocs extension
        /// </returns>
        [PublicAPI, NotNull]
        IExtension Create([NotNull] IMarkDocService service, [NotNull] IMarkDocServiceState state);
    }
}