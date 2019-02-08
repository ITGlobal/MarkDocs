using ITGlobal.MarkDocs.Source;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITGlobal.MarkDocs.Format.Impl.Metadata
{
    internal static class PageAnchorReader
    {
        public static PageAnchor[] Read(MarkdownDocument document)
        {
            var headings = document.Descendants<HeadingBlock>().ToArray();

            var tree = GenerateTocTree(headings);
            return tree.ToPageAnchor().Nested;
        }

        private sealed class TocTreeNode
        {
            private readonly List<TocTreeNode> _children = new List<TocTreeNode>();

            private TocTreeNode(HeadingBlock heading)
            {
                Id = heading?.GetAttributes().Id ?? "";
                Title = heading?.Inline.GetText() ?? "";
                Level = heading?.Level ?? 0;
            }

            private string Id { get; }
            private string Title { get; }
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
                return new PageAnchor(
                    id: Id,
                    title: Title,
                    nested: _children.Count > 0 ? _children.Select(_ => _.ToPageAnchor()).ToArray() : null
                );
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                Print(sb);
                return sb.ToString();
            }

            private void Print(StringBuilder sb)
            {
                for (var i = 0; i < Level; i++)
                {
                    sb.Append("    ");
                }

                sb.Append("\"");
                sb.Append(Title);
                sb.Append("\" {#");
                sb.Append(Id);
                sb.AppendLine("}");
                foreach (var c in _children)
                {
                    c.Print(sb);
                }
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
                    while (roots.Count > 0&& relativeDepth < 0)
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