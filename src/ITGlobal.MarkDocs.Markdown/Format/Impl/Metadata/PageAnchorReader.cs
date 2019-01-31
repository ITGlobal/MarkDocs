using System.Collections.Generic;
using System.Linq;
using ITGlobal.MarkDocs.Source;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Metadata
{
    internal static class PageAnchorReader
    {
        public static PageAnchor[] Read(MarkdownDocument document)
        {
            var headings = document.OfType<HeadingBlock>().ToArray();

            var tree = GenerateTocTree(headings);
            return tree.ToPageAnchor().Nested;
        }

        private sealed class TocTreeNode 
        {
            private readonly List<TocTreeNode> _children = new List<TocTreeNode>();

            private TocTreeNode(HeadingBlock heading)
            {
                Heading = heading;
                Level = heading?.Level ?? 0;
            }

            private HeadingBlock Heading { get; }
            public int Level { get; }
            
            public TocTreeNode AppendChild(HeadingBlock heading)
            {
                var child = new TocTreeNode(heading);
                _children.Add(child);
                return child;
            }

            public static TocTreeNode Root() => new TocTreeNode(null);

            public PageAnchor ToPageAnchor()
            {
                return new PageAnchor
                {
                    Id = Heading?.GetAttributes().Id ?? "",
                    Title = Heading?.Inline.GetText() ?? "",
                    Nested = _children.Count>0? _children.Select(_=>_.ToPageAnchor()).ToArray(): null
                };
            }
        }

        private static TocTreeNode GenerateTocTree(HeadingBlock[] headings)
        {
            var root = TocTreeNode.Root();
            var currentRoot = root;
            var roots = new Stack<TocTreeNode>();
            var lastNode = root;

            for (var index = 0; index < headings.Length; index++)
            {
                var heading = headings[index];
                var relativeDepth = heading.Level - (currentRoot.Level + 1);

                if (relativeDepth == 0)
                {
                    // We are at the same depth as before
                    lastNode = currentRoot.AppendChild(heading);
                    continue;
                }

                if (relativeDepth < 0)
                {
                    // End of nested headings
                    while (relativeDepth < 0)
                    {
                        currentRoot = roots.Pop();
                        relativeDepth = heading.Level - (currentRoot.Level + 1);
                    }

                    roots.Push(currentRoot);

                    lastNode = currentRoot.AppendChild(heading);
                    continue;
                }

                if (relativeDepth >= 1)
                {
                    // We have e.g. H3 nested into H1
                    // That's not consistent!
                    // We will fix all nested headings to ensure everything is OK
                    FixInconsistentHeadings(headings, index, relativeDepth - 1);
                }

                // Start of nested headings
                roots.Push(lastNode);
                currentRoot = lastNode;
                lastNode = currentRoot.AppendChild(heading);
            }

            return root;
        }

        private static void FixInconsistentHeadings(HeadingBlock[] headings, int index, int amount)
        {
            var level = headings[index].Level;

            for (; index < headings.Length && headings[index].Level >= level; index++)
            {
                headings[index].Level -= amount;
            }
        }
    }
}