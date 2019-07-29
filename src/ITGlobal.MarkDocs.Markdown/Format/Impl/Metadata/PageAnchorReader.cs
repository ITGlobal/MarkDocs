using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using ITGlobal.MarkDocs.Source;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITGlobal.MarkDocs.Format.Impl.Metadata
{
    internal static class PageAnchorReader
    {
        private static readonly HtmlParser _HtmlParser = new HtmlParser();

        public static PageAnchor[] Read(MarkdownDocument document)
        {
            var anchors = new Dictionary<string, PageAnchor>();

            var headings = document.Descendants<HeadingBlock>().ToArray();
            var tree = GenerateTocTree(headings);
            foreach (var anchor in tree.ToPageAnchor().Nested)
            {
                anchors[anchor.Id] = anchor;
            }

            foreach (var htmlBlock in document.Descendants<HtmlBlock>())
            {
                ProcessHtmlNode(htmlBlock.Lines.ToString());
            }
            foreach (var htmlInline in document.Descendants().OfType<HtmlInline>())
            {
                ProcessHtmlNode(htmlInline.Tag);
            }

            return anchors.Values.OrderBy(_ => _.Id).ToArray();

            void ProcessHtmlNode(string markup)
            {
                try
                {
                    var html = _HtmlParser.ParseDocument(markup);
                    foreach (var domNode in html.All)
                    {
                        if (!string.IsNullOrEmpty(domNode.Id))
                        {
                            var anchor = new PageAnchor(domNode.Id, domNode.Text(), Array.Empty<PageAnchor>());
                            anchors[anchor.Id] = anchor;
                        }

                        if (domNode.TagName == "A")
                        {
                            var name = domNode.GetAttribute("name");
                            if (!string.IsNullOrEmpty(name))
                            {
                                var anchor = new PageAnchor(name, domNode.Text(), Array.Empty<PageAnchor>());
                                anchors[anchor.Id] = anchor;
                            }
                        }
                    }
                }
                catch { }
            }
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
                    while (roots.Count > 0 && relativeDepth < 0)
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