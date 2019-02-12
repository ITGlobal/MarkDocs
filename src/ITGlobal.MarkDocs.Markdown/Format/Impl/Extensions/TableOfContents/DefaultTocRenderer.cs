using System.Collections.Generic;
using ITGlobal.MarkDocs.Source;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.TableOfContents
{
    internal sealed class DefaultTocRenderer : ITocRenderer
    {
        public void Render(HtmlRenderer renderer, IReadOnlyList<PageAnchor> anchors)
        {
            renderer.WriteLine("<ul>");
            for (var index = 0; index < anchors.Count; index++)
            {
                Render(renderer, anchors[index], index, 1);
            }

            renderer.WriteLine("</ul>");
        }

        private void Render(HtmlRenderer renderer, PageAnchor anchor, int index, int level)
        {
            // Don't render a <li> for
            // * root node
            // * first H1 node (first H1 is a page title and should not be a part of TOC)
            var shouldRenderLi = level > 1 || level == 1 && index > 0;

            if (shouldRenderLi)
            {
                renderer.WriteLine("<li>");
                renderer.Write("<a href=\"#");
                renderer.WriteEscape(anchor.Id);
                renderer.Write("\">");
                renderer.WriteEscape(anchor.Title);
                renderer.WriteLine("</a>");
            }

            if (anchor.Nested?.Length > 0)
            {
                renderer.WriteLine("<ul>");
                for (var i = 0; i < anchor.Nested.Length; i++)
                {
                    Render(renderer, anchor.Nested[i], i, level+1);
                }
                renderer.WriteLine("</ul>");
            }

            if (shouldRenderLi)
            {
                renderer.WriteLine("</li>");
            }
        }
    }
}