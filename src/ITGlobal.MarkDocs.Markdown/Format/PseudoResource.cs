using System.Threading;
using ITGlobal.MarkDocs.Content;

namespace ITGlobal.MarkDocs.Format
{
    internal sealed class PseudoResource : IResource
    {
        private static readonly ThreadLocal<PseudoResource> LocalInstance = new ThreadLocal<PseudoResource>(
            () => new PseudoResource());

        private PseudoResource() { }

        public static IResource Get(IDocumentation documentation, string id, string filename, ResourceType type)
        {
            ResourceId.Normalize(ref id);

            var resource = LocalInstance.Value;
            resource.Documentation = documentation;
            resource.Id = id;
            resource.FileName = filename;
            resource.Type = type;

            return resource;
        }

        public string Id { get; private set; }

        public IDocumentation Documentation { get; private set; }

        public string FileName { get; private set; }

        public ResourceType Type { get; private set; }
    }
}