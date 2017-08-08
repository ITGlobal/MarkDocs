using System.Collections.Generic;
using System.Linq;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Format.ChildrenList
{
    internal sealed class DefaultChildrenListRenderer : IChildrenListRenderer
    {
        public void Render(HtmlRenderer renderer, IPage page)
        {
            IEnumerable<IPageTreeNode> nodes = null;

            var pageNode = page.PageTreeNode;
            if (pageNode.Nodes != null)
            {
                nodes = pageNode.Nodes;
            }
            else if (pageNode.Parent != null)
            {
                nodes = pageNode.Parent.Nodes?.Where(_ => _.Id == pageNode.Id);
            }

            if (nodes == null)
            {
                return;
            }

            renderer.WriteLine("<ul>");

            foreach (var node in nodes)
            {
                var url = "#";
                if (node.IsHyperlink && MarkdownRenderingContext.ResourceUrlResolver != null)
                {
                    url = MarkdownRenderingContext.ResourceUrlResolver.ResolveUrl(node, MarkdownRenderingContext.RenderContext.Page);
                }

                renderer.WriteLine("<li>");
                renderer.Write($"<a href=\"{url}\">{node.Title}</a>");
                renderer.WriteLine("</li>");
            }
            renderer.WriteLine("</ul>");
        }
    }
}