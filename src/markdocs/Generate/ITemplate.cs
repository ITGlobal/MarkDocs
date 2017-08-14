using System.IO;
using ITGlobal.MarkDocs.Cache;

namespace ITGlobal.MarkDocs.Tools.Generate
{
    public interface ITemplate
    {
        string Name { get; }
        void Initialize(ICacheUpdateOperation operation, IDocumentation documentation);
        void Render(IPage page, string content, TextWriter writer);
    }
}