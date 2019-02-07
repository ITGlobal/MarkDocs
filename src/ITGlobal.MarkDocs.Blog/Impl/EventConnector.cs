using ITGlobal.MarkDocs.Extensions;

namespace ITGlobal.MarkDocs.Blog.Impl
{
    internal sealed class EventConnector : IExtensionFactory, IExtension
    {
        public BlogEngineImpl Engine { get; set; }

        public IExtension Create(IMarkDocService service) => this;

        public void Initialize(IMarkDocState state) => Engine?.Update(state);

        public void OnCreated(IDocumentation documentation) { }

        public void OnUpdated(IDocumentation documentation) => Engine?.Update(documentation);

        public void OnUpdateCompleted(IDocumentation documentation) { }

        public void OnRemoved(IDocumentation documentation) { }
    }
}