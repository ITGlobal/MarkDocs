using System;
using System.Linq;
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
                var i = 0;
                RenderTree(renderer, headings, ref i, 1);
            }
            catch (Exception exception)
            {
                Logger.LogError(0, exception, "Error while rendering {0}", nameof(TableOfContentsBlock));
                renderer.WriteError("Failed to render table of contents");
            }
        }

        private static void RenderTree(HtmlRenderer renderer, HeadingBlock[] headings, ref int i, int currentLevel)
        {
            var hasAnyItems = false;
            var closeLi = false;
            var skipFirstL1 = true;

            for (; i < headings.Length; i++)
            {
                var h = headings[i];

                if (h.Level > currentLevel)
                {
                    RenderTree(renderer, headings, ref i, currentLevel + 1);
                    continue;
                }

                if (h.Level < currentLevel)
                {
                    break;
                }

                if (h.Level == 1 && skipFirstL1)
                {
                    // First H1 is a page title and should not be a part of TOC
                    skipFirstL1 = false;
                    continue;
                }
                
                if (!hasAnyItems)
                {
                    renderer.WriteLine("<ul>");
                    hasAnyItems = true;
                }

                if (closeLi)
                {
                    renderer.WriteLine("</li>");
                    closeLi = false;
                }

                renderer.WriteLine("<li>");
                var id = h.GetAttributes().Id;
                var text = h.Inline.GetText();

                renderer.Write($"<a href=\"#{id}\">{text}</a>");

                closeLi = true;
            }

            if (closeLi)
            {
                renderer.WriteLine("</li>");
            }

            if (hasAnyItems)
            {
                renderer.WriteLine("</ul>");
            }
        }
    }
}