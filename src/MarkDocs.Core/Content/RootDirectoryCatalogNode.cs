using System.Collections.Generic;
using ITGlobal.MarkDocs.Format;

namespace ITGlobal.MarkDocs.Content
{
    /// <summary>
    ///     Root directory page tree node
    /// </summary>
    internal sealed class RootDirectoryPageTreeNode : DirectoryPageTreeNode, IPageTree
    {
        /// <summary>
        ///     .ctor
        /// </summary>
        public RootDirectoryPageTreeNode(
            string id, 
            Metadata metadata, 
            string rootDirectory, 
            string filename, 
            string relativeFileName, 
            IReadOnlyList<IPageTreeNode> nodes,
            string[] attachments)
            : base(id, metadata, filename, relativeFileName, nodes)
        {
            RootDirectory = rootDirectory;
            Attachments = attachments;

            var pages = new List<IPageTreeNode>();
            ScanNodes((node, _) =>
            {
                if (node.IsHyperlink)
                {
                    pages.Add(node);
                }
            });

            Pages = pages;
        }

        /// <summary>
        ///     Absolute path to root directory
        /// </summary>
        public string RootDirectory { get; }

        /// <summary>
        ///     List of attachments
        /// </summary>
        public string[] Attachments { get; }

        /// <summary>
        ///     Flat list of pages
        /// </summary>
        public IReadOnlyList<IPageTreeNode> Pages { get; }
    }
}