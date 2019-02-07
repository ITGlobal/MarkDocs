using System.Collections.Generic;
using System.IO;
using ITGlobal.MarkDocs.Cache;

namespace ITGlobal.MarkDocs.Tools.Generate
{
    public sealed class PreviewTemplate : ITemplate
    {
        private const string Css = @"
.page-layout-root {
    position: absolute;
    left: 0;
    top: 53px;
    right: 0;
    bottom: 0;
}

    .page-layout-root > .page-breadcrumbs {
        position: absolute;
        left: 0;
        top: 0;
        right: 0;
        height: 30px;
    }

    .page-layout-root > .page-sidebar {
        position: absolute;
        left: 0;
        top: 30px;
        width: 350px;
        bottom: 0;
        overflow-y: scroll;
    }

    .page-layout-root > .page-content {
        position: absolute;
        left: 360px;
        top: 30px;
        right: 0;
        bottom: 0;
        overflow-y: scroll;
    }
hr {
    border-color: black;
    width: 100%;
}

.list-group * {
    word-break: break-all;
}

.page-tree {
    list-style-type: none;
    padding-left: 0;
}

    .page-tree a {
        display: block;
    }

    .page-tree ul {
        list-style-type: none;
    }

    .page-tree li {
        color: #337ab7;
    }

        .page-tree li.active > a {
            color: #fff;
            background-color: #337ab7;
        }
";
        public string Name => "preview";

        public void Initialize(ICacheUpdateTransaction transaction) { }

        public void Render(IPage page, string content, TextWriter writer)
        {
            writer.WriteLine("<!DOCTYPE html>");
            writer.WriteLine("<html lang=\"en\">");
            writer.WriteLine("<head>");
            writer.WriteLine("  <meta charset=\"utf-8\">");
            writer.WriteLine("  <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">");
            writer.WriteLine("  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
            writer.WriteLine($"  <title>{page.Title}</title>");
            writer.WriteLine($"  <style>{Css}</style>");
            writer.WriteLine("  <link href=\"https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.3.7/css/bootstrap.min.css\" rel=\"stylesheet\">");
            writer.WriteLine("  <link href=\"https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css\" rel=\"stylesheet\">");
            writer.WriteLine("  <link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/highlight.js/9.7.0/styles/vs.min.css\" />");
            writer.WriteLine("  <link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/mermaid/6.0.0/mermaid.min.css\" />");
            writer.WriteLine("  <link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/mermaid/6.0.0/mermaid.forest.css\" />");
            writer.WriteLine("  <!--[if lt IE 9]>");
            writer.WriteLine("    <script src=\"https://cdnjs.cloudflare.com/ajax/libs/html5shiv/3.7.3/html5shiv.min.js\"></script>");
            writer.WriteLine("    <script src=\"https://cdnjs.cloudflare.com/ajax/libs/respond.js/1.4.2/respond.min.js\"></script>");
            writer.WriteLine("  <![endif]-->");
            writer.WriteLine("</head>");
            writer.WriteLine("<body>");
            writer.WriteLine("  <div class=\"navbar navbar-inverse navbar-fixed-top\">");
            writer.WriteLine("    <div class=\"container\">");
            writer.WriteLine("      <div class=\"navbar-header\">");
            var url = OutputCache.GetRelativeResourcePath(page.Documentation.GetPage("/"), page);
            writer.WriteLine($"        <a href=\"{url}\" class=\"navbar-brand\">MarkDocs</a>");
            writer.WriteLine("      </div>");
            writer.WriteLine("    </div>");
            writer.WriteLine("  </div>");
            writer.WriteLine("  <div class=\"container\">");
            writer.WriteLine("    <div class=\"page-layout-root\">");
            writer.WriteLine("      <div class=\"page-breadcrumbs\">");
            RenderBreadcrumbs(writer, page);
            writer.WriteLine("      </div>");
            writer.WriteLine("      <div class=\"page-sidebar\">");
            RenderPageTree(writer, page);
            writer.WriteLine("      </div>");
            writer.WriteLine("      <div class=\"page-content\">");
            writer.WriteLine(content);
            writer.WriteLine("      </div>");
            writer.WriteLine("    </div>");
            writer.WriteLine("  </div>");
            writer.WriteLine("  <script src=\"https://cdnjs.cloudflare.com/ajax/libs/jquery/3.1.1/jquery.min.js\"></script>");
            writer.WriteLine("  <script src=\"https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.3.7/js/bootstrap.min.js\"></script>");
            writer.WriteLine("  <script type=\"text/javascript\" src=\"https://cdnjs.cloudflare.com/ajax/libs/highlight.js/9.7.0/highlight.min.js\"></script>");
            writer.WriteLine("  <script type=\"text/javascript\" src=\"https://cdnjs.cloudflare.com/ajax/libs/mermaid/6.0.0/mermaid.min.js\"></script>");
            writer.WriteLine("  <script>hljs.initHighlightingOnLoad();</script>");
            writer.WriteLine("  <script>$(document).load(function() { mermaid.initialize(); });</script>");
            writer.WriteLine("  ");
            writer.WriteLine("</body>");
            writer.WriteLine("</html>");
        }

        private static void RenderBreadcrumbs(TextWriter writer, IPage page)
        {
            var nodes = new Stack<IPage>();

            var n = page;
            while (n != null)
            {
                nodes.Push(n);
                n = n.Parent;
            }

            writer.WriteLine("<ol class=\"breadcrumb\">");
            while (nodes.Count > 0)
            {
                var node = nodes.Pop();
                var url = OutputCache.GetRelativeResourcePath(node, page);
                var isActive = node.Id == page.Id;

                if (isActive)
                {
                    writer.WriteLine("<li class=\"active\">");
                }
                else
                {
                    writer.WriteLine("<li>");
                }

                writer.WriteLine($"<a href=\"{url}\">{node.Title}</a>");
                writer.WriteLine("</li>");
            }
            writer.WriteLine("</ol>");
        }

        private static void RenderPageTree(TextWriter writer, IPage page)
        {
            writer.WriteLine("<ul class=\"md-page-tree\">");

            var isActive = page.Id == "/";

            if (isActive)
            {
                writer.WriteLine("<li class=\"active\">");
            }
            else
            {
                writer.WriteLine("<li>");
            }

            var url = OutputCache.GetRelativeResourcePath(page.Documentation.GetPage("/"), page);

            writer.WriteLine($"<a href=\"{url}\">");
            writer.WriteLine("   <i class=\"fa fa-fw fa-home\"></i>");
            writer.WriteLine("   <span>Home</span>");
            writer.WriteLine("</a>");

            writer.WriteLine("</li>");

            foreach (var node in page.Documentation.RootPage.NestedPages)
            {
                RenderPageTreeNode(writer, page, node);
            }

            writer.WriteLine("</ul>");
        }

        private static void RenderPageTreeNode(TextWriter writer, IPage refPage, IPage node)
        {
            var url = OutputCache.GetRelativeResourcePath(node, refPage) ;

            var isFolder = node.NestedPages.Length>0;
            var icon = isFolder ? "<i class=\"fa fa-fw fa-folder-o\"></i>" : "<i class=\"fa fa-fw fa-file-o\"></i>";
            var isActive = refPage.Id == node.Id;

            if (isActive)
            {
                writer.WriteLine("<li class=\"active\">");
            }
            else
            {
                writer.WriteLine("<li>");
            }

            if (url != null)
            {
                writer.WriteLine($"<a href=\"{url}\">{icon} <span>{node.Title}</span></a>");
            }
            else
            {
                writer.WriteLine($"{icon} <span>{node.Title}</span>");
            }

            if (isFolder)
            {
                writer.WriteLine("<ul>");
                foreach (var child in node.NestedPages)
                {
                    RenderPageTreeNode(writer, refPage, child);

                }
                writer.WriteLine("</ul>");
            }

            writer.WriteLine("</li>");
        }
    }
}