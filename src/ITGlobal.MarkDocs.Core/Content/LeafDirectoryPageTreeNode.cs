using ITGlobal.MarkDocs.Format;

namespace ITGlobal.MarkDocs.Content
{
    /// <summary>
    ///     Leaf directory tree node (a directory that contains only an index page file)
    /// </summary>
    internal sealed class LeafDirectoryPageTreeNode : LeafPageTreeNode
    {
        /// <summary>
        ///     .ctor
        /// </summary>
        public LeafDirectoryPageTreeNode(string id, Metadata metadata, string filename, string relativeFileName)
            : base(id, metadata, filename, relativeFileName)
        { }

        /// <summary>
        ///     true if this node refers to an index page
        /// </summary>
        public override bool IsIndexPage => true;
    }
}