using System.Collections.Generic;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using System.Linq;

namespace ITGlobal.MarkDocs.Format.ChildrenList
{
    internal sealed class ChildrenListBlockRenderer : HtmlObjectRenderer<ChildrenListBlock>
    {
        private readonly IResourceUrlResolver _resourceUrlResolver;

        public ChildrenListBlockRenderer(IResourceUrlResolver resourceUrlResolver)
        {
            _resourceUrlResolver = resourceUrlResolver;
        }

        protected override void Write(HtmlRenderer renderer, ChildrenListBlock block)
        {
            IEnumerable<IPageTreeNode> nodes = null;

            var refPage = block.Page;
            var pageNode = block.Page.PageTreeNode;
            if (pageNode.Nodes != null)
            {
                nodes = pageNode.Nodes;
            }
            else if (pageNode.Parent != null)
            {
                refPage = refPage.Documentation.GetPage(pageNode.Parent.Id);
                nodes = pageNode.Parent.Nodes?.Where(_ => _.Id == pageNode.Id);
            }

            if (nodes == null)
            {
                return;
            }

            renderer.WriteLine("<div class=\"panel panel-primary\">");
            renderer.WriteLine("  <div class=\"panel-heading\">Child pages</div>");
            renderer.WriteLine("  <div class=\"list-group\">");
            foreach (var node in nodes)
            {
                var url = "#";
                if (node.IsHyperlink)
                {
                    url = _resourceUrlResolver.ResolveUrl(refPage, "/" + node.Id);
                }
                
                renderer.Write($"   <a href=\"{url}\" class=\"list-group-item\">{node.Title}</a>");
            }
            renderer.WriteLine("  </div>");
            renderer.WriteLine("</div>");
        }
    }
}
