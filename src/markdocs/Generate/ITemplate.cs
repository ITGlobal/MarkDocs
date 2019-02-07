using System.IO;
using ITGlobal.MarkDocs.Cache;

namespace ITGlobal.MarkDocs.Tools.Generate
{
    public interface ITemplate
    {
        string Name { get; }
        void Initialize(ICacheUpdateTransaction transaction);
        void Render(IPage page, string content, TextWriter writer);
    }
}