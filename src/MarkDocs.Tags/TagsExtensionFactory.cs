using ITGlobal.MarkDocs.Extensions;

namespace ITGlobal.MarkDocs.Tags
{
    /// <summary>
    ///     A factory for tags extension
    /// </summary>
    internal sealed class TagsExtensionFactory : IExtensionFactory
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
        public IExtension Create(IMarkDocService service, IMarkDocServiceState state) => new TagsExtension(state);
    }
}