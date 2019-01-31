using Markdig.Parsers;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.TableOfContents
{
    internal sealed class TableOfContentsBlockParser : BlockParser
    {
        public TableOfContentsBlockParser()
        {
            OpeningCharacters = new[] {':'};
        }

        public override BlockState TryOpen(BlockProcessor processor)
        {
            if (processor.Line.Length < 9)
            {
                return BlockState.None;
            }

            // TODO use better matching
            var line = processor.Line.ToString().Trim();
            if (line != ":::toc:::")
            {
                return BlockState.None;
            }

            var sourcePosition = processor.Start;
            processor.NewBlocks.Push(new TableOfContentsBlock(this, processor.Document)
            {
                Column = processor.Column,
                Span = new SourceSpan(sourcePosition, processor.Line.End)
            });

            return BlockState.BreakDiscard;
        }
    }
}