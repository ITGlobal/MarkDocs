using System.Threading;

namespace ITGlobal.MarkDocs.Format
{
    internal sealed class PseudoResource : IResource
    {
        private static readonly ThreadLocal<PseudoResource> LocalInstance = new ThreadLocal<PseudoResource>(
            () => new PseudoResource());

        private PseudoResource() { }

        public static IResource Get(IDocumentation documentation, string id)
        {
            var resource = LocalInstance.Value;
            resource.Documentation = documentation;
            resource.Id = id;
            return resource;
        }

        public string Id { get; private set; }

        public IDocumentation Documentation { get; private set; }
    }
}