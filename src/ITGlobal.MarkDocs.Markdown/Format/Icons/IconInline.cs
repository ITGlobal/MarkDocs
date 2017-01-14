using Markdig.Syntax.Inlines;

namespace ITGlobal.MarkDocs.Format.Icons
{
    internal sealed class IconInline : LeafInline
    {
        public IconInline(string type, string id)
        {
            Type = type;
            Id = id;
        }
        
        public string Type { get;  }
        public string Id { get; }
    }
}
