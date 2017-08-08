using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Microsoft.Extensions.Logging;
using static ITGlobal.MarkDocs.Format.MarkdownRenderingContext;

namespace ITGlobal.MarkDocs.Format.TableOfContents
{
    internal sealed class TableOfContentsBlockRenderer : HtmlObjectRenderer<TableOfContentsBlock>
    {
        protected override void Write(HtmlRenderer renderer, TableOfContentsBlock block)
        {
            try
            {
                var document = block.Document;

                var headings = document.OfType<HeadingBlock>().ToArray();

                var tree = GenerateTocTree(headings);
                
                var tocRenderer = TocRenderer;
                if (tocRenderer != null)
                {
                    tocRenderer.Render(renderer, tree);
                }
                else
                {
                    tree.Render(renderer);
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(0, exception, "Error while rendering {0}", nameof(TableOfContentsBlock));
                renderer.WriteError("Failed to render table of contents");
            }
        }

        private sealed class TocTreeNode : ITocTreeNode
        {
            private readonly List<ITocTreeNode> children = new List<ITocTreeNode>();
            
            private TocTreeNode(HeadingBlock heading)
            {
                Heading = heading;
            }

            public HeadingBlock Heading { get; }
            public string Title => Heading?.Inline.GetText() ?? "";
            public int Level => Heading?.Level ?? 0;

            public IReadOnlyList<ITocTreeNode> Children => children;

            public TocTreeNode AppendChild(HeadingBlock heading)
            {
                var child = new TocTreeNode(heading);
                children.Add(child);
                return child;
            }

            public static TocTreeNode Root() => new TocTreeNode(null);

            public override string ToString()
            {
                var sb = new StringBuilder();
                ToString(sb);
                return sb.ToString();
            }

            private void ToString(StringBuilder sb)
            {
                if (Heading != null)
                {
                    for (var i = 0; i < Level; i++)
                    {
                        sb.Append("  ");
                    }
                    sb.Append(Title);
                    sb.AppendLine();
                }

                foreach (TocTreeNode child in Children)
                {
                    child.ToString(sb);
                }
            }

            public void Render(HtmlRenderer renderer) => Render(renderer, 0);

            private void Render(HtmlRenderer renderer, int index)
            {
                if (Level == 0 && Children.Count == 1)
                {
                    // This is a root (dummy) node and it contains only one H1 node
                    // We should short-circuit rendering to this H1 node to avoid
                    // rendering of extra outer <ul>
                    ((TocTreeNode)Children[0]).Render(renderer, 0);
                    return;
                }

                // Don't render a <li> for
                // * root node
                // * first H1 node (first H1 is a page title and should not be a part of TOC)
                var shouldRenderLi = Level > 1 || Level == 1 && index > 0;

                if (shouldRenderLi)
                {
                    renderer.WriteLine("<li>");
                    var id = Heading.GetAttributes().Id;
                    var text = Heading.Inline.GetText();

                    renderer.Write($"<a href=\"#{id}\">{text}</a>");
                }

                if (Children.Count > 0)
                {
                    renderer.WriteLine("<ul>");
                    for (var i = 0; i < Children.Count; i++)
                    {
                        var child = (TocTreeNode)Children[i];
                        child.Render(renderer, i);
                    }
                    renderer.WriteLine("</ul>");
                }

                if (shouldRenderLi)
                {
                    renderer.WriteLine("</li>");
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