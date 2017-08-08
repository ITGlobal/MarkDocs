using Markdig.Syntax.Inlines;

namespace ITGlobal.MarkDocs.Format
{
    public sealed class IconInline : LeafInline
    {
        internal IconInline(string type, string id)
        {
            Type = type;
            Id = id;
        }
        
        public string Type { get;  }
        public string Id { get; }
    }
}
