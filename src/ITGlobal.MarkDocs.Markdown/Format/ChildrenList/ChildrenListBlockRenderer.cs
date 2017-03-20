using System;
using System.Collections.Generic;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using System.Linq;
using Microsoft.Extensions.Logging;
using static ITGlobal.MarkDocs.Format.MarkdownRenderingContext;

namespace ITGlobal.MarkDocs.Format.ChildrenList
{
    internal sealed class ChildrenListBlockRenderer : HtmlObjectRenderer<ChildrenListBlock>
    {
        protected override void Write(HtmlRenderer renderer, ChildrenListBlock block)
        {
            try
            {
                IEnumerable<IPageTreeNode> nodes = null;
                
                var pageNode = block.Page.PageTreeNode;
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
                    if (node.IsHyperlink && ResourceUrlResolver != null)
                    {
                        url = ResourceUrlResolver.ResolveUrl(node, RenderContext.Page);
                    }

                    renderer.WriteLine("<li>");
                    renderer.Write($"<a href=\"{url}\">{node.Title}</a>");
                    renderer.WriteLine("</li>");
                }
                renderer.WriteLine("</ul>");
            }
            catch (Exception exception)
            {
                Logger.LogError(0, exception, "Error while rendering {0}", nameof(ChildrenListBlock));
                renderer.WriteError("Failed to render children list");
            }
        }
    }
}
