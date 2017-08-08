using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ITGlobal.MarkDocs.Extensions;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using Lucene.Net.Store;
using Lucene.Net.Index;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Search;
using System.Linq;
using Lucene.Net.Search.Spell;
using Lucene.Net.Util;

namespace ITGlobal.MarkDocs.Search
{

    internal static class PageRenderer
    {
        public static string RenderPlainText(this IPage page)
        {
            var html = page.ReadHtmlString();

            var parser = new HtmlParser();
            var document = parser.Parse(html);

            var text = new StringBuilder();
            RenderHtmlRec(document, text);
            return text.ToString();
        }

        private static void RenderHtmlRec(INode node, StringBuilder text)
        {
            var textNode = node as IText;
            if (textNode != null)
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
