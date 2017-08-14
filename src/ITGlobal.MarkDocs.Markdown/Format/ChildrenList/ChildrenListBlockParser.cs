using Markdig.Parsers;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.ChildrenList
{
    internal sealed class ChildrenListBlockParser : BlockParser
    {
        public ChildrenListBlockParser()
        {
            OpeningCharacters = new[] { ':' };
        }

        public override BlockState TryOpen(BlockProcessor processor)
        {
            if (!MarkdownParseContext.IsPresent)
            {
                return BlockState.None;
            }

            if (processor.Line.Length < 9)
            {
                return BlockState.None;
            }

            // TODO use better matching
            var line = processor.Line.ToString().Trim();
            if (line != ":::children:::")
            {
                return BlockState.None;
            }

            var sourcePosition = processor.Start;
            processor.NewBlocks.Push(new ChildrenListBlock(this, processor.Document, MarkdownParseContext.ParseContext.Page)
            {
                Column = processor.Column,
                Span = new SourceSpan(sourcePosition, processor.Line.End)
            });

            return BlockState.BreakDiscard;
        }
    }
}