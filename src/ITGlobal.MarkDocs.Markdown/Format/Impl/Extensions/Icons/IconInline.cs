using Markdig.Syntax.Inlines;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.Icons
{
    /// <summary>
    ///     An icon inline
    /// </summary>
    public sealed class IconInline : LeafInline
    {
        internal IconInline(string type, string id)
        {
            Type = type;
            Id = id;
        }

        /// <summary>
        ///     Icon type
        /// </summary>
        public string Type { get;  }

        /// <summary>
        ///     Icon id
        /// </summary>
        public string Id { get; }
    }
}
