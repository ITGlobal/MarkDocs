using System.Linq;
using Markdig.Extensions.Mathematics;
using Markdig.Helpers;
using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace ITGlobal.MarkDocs.Format.Impl
{
    internal static class MarkdownHelper
    {
        public static string GetText(this Inline inline)
        {
            switch (inline)
            {
                case LiteralInline literal:
                    return literal.Content.ToString();

                case MathInline mathInline:
                    return mathInline.Content.ToString();

                case ContainerInline container:
                    return string.Concat(container.FindDescendants<LiteralInline>().Select(GetText));

                default:
                    return null;
            }
        }

        public static string GetText(this LeafBlock block)
        {
            var sb = StringBuilderCache.Local();
            if (block.Lines.Lines != null)
            {
                var lines = block.Lines.Lines;
                for (var index = 0; index < block.Lines.Count; ++index)
                {
                    var slice = lines[index].Slice;
                    if (slice.Start <= slice.End)
                    {
                        var end = slice.Start + slice.Length;
                        for (var i = slice.Start; i < end; i++)
                        {
                            sb.Append(slice.Text[i]);
                        }
                    }

                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        public static MarkdownDocument GetDocument(this Block block)
        {
            while (block != null && !(block is MarkdownDocument))
            {
                block = block.Parent;
            }

            return block as MarkdownDocument;
        }

        public static void WriteErrorInline(this HtmlRenderer renderer, string message)
        {
            renderer.Write("<span style=\"background: white; color: red\"><strong>Error!</strong> ");
            renderer.WriteEscape(message);
            renderer.Write("</span>");
        }

        public static void WriteError(this HtmlRenderer renderer, string message)
        {
            renderer.Write("<div style=\"background: white; color: red\"><strong>Error!</strong> ");
            renderer.WriteEscape(message);
            renderer.Write("</div>");
        }
    }
}