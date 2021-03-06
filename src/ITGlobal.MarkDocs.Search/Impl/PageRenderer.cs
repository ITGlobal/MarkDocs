﻿using System.Text;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;

namespace ITGlobal.MarkDocs.Search.Impl
{
    internal static class PageRenderer
    {
        public static string RenderPlainText(this IPage page)
        {
            var html = page.ReadHtmlString();

            var parser = new HtmlParser();
            var document = parser.ParseDocument(html);

            var text = new StringBuilder();
            RenderHtmlRec(document, text);
            return text.ToString();
        }

        private static void RenderHtmlRec(INode node, StringBuilder text)
        {
            if (node is IText textNode)
            {
                text.AppendLine(textNode.Text);
            }

            if (node.HasChildNodes)
            {
                foreach (var childNode in node.ChildNodes)
                {
                    RenderHtmlRec(childNode, text);
                }
            }
        }
    }
}
