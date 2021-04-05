using System.Linq;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.Icons
{
    internal sealed class IconParser : InlineParser
    {
        private readonly IconType[] _iconTypes = { new IconType("fa fa-fw", "fa-") };

        public IconParser()
        {
            OpeningCharacters = new[] { ':' };
        }

        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            if (slice.Length < 3)
            {
                return false;
            }

            var startPosition = slice.Start;
            var builder = StringBuilderCache.Local();

            var c = slice.NextChar();
            while (!slice.IsEmpty)
            {
                if (c == ':')
                {
                    break;
                }

                if (!char.IsLetterOrDigit(c) && c != '-' && c != '_')
                {
                    break;
                }

                builder.Append(c);
                c = slice.NextChar();
            }

            slice.NextChar();

            if (builder.Length == 0)
            {
                return false;
            }

            var id = builder.ToString().Trim();
            var iconType = _iconTypes.FirstOrDefault(type => id.StartsWith(type.Prefix));
            if (iconType == null)
            {
                return false;
            }
                
            int line;
            int column;

            var start = processor.GetSourcePosition(startPosition, out line, out column);
            var end = processor.GetSourcePosition(slice.Start);
            processor.Inline = new IconInline(iconType.Type, id)
            {
                Span = new SourceSpan(start, end),
                Line = line,
                Column = column
            };
                
            return true;
        }
    }
}