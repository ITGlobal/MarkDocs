using ITGlobal.MarkDocs.Extensions;

namespace ITGlobal.MarkDocs.Tags.Impl
{
    /// <summary>
    ///     A factory for tags extension
    /// </summary>
    internal sealed class TagsExtensionFactory : IExtensionFactory
    {
        public IExtension Create(IMarkDocService service) => new TagsExtension();
    }
}