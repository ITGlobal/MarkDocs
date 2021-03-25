using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using ITGlobal.MarkDocs.Format.Impl;
using JetBrains.Annotations;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     A parsed HTML block
    /// </summary>
    [PublicAPI]
    public sealed class ParsedHtmlBlock : HtmlBlock
    {

        private static readonly HtmlParser HtmlParser = new HtmlParser();

        /// <summary>
        ///     .ctor
        /// </summary>
        private ParsedHtmlBlock(HtmlBlock block, IElement element)
            : base(block.Parser)
        {
            Block = block;
            Element = element;
        }

        /// <summary>
        ///     Raw block
        /// </summary>
        [NotNull]
        public HtmlBlock Block { get; }

        /// <summary>
        ///     HTML markup element
        /// </summary>
        [NotNull]
        public IElement Element { get; }

        [CanBeNull]
        internal static ParsedHtmlBlock TryCreate(HtmlBlock htmlBlock)
        {
            try
            {
                var html = htmlBlock.GetText();
                html = $"<html><body>{html}</body></html>";
                var document = HtmlParser.ParseDocument(html);
                var element = document.Body?.FirstElementChild;
                if (element == null)
                {
                    return null;
                }

                return new ParsedHtmlBlock(htmlBlock, element);
            }
            catch
            {
                return null;
            }
        }

    }
}