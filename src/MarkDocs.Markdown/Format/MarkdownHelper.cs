using System.Linq;
using Markdig.Syntax.Inlines;

namespace ITGlobal.MarkDocs.Format
{
    internal static class MarkdownHelper
    {
        public static string GetText(this Inline inline)
        {
            var literal = inline as LiteralInline;
            if (literal != null)
            {
                return literal.Content.ToString();
            }
            var container = inline as ContainerInline;
            if (container != null)
            {
                return string.Concat(container.FindDescendants<LiteralInline>().Select(GetText));
            }

            return null;
        }
    }
}