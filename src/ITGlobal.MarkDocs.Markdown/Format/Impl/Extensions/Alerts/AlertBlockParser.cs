using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.Alerts
{
    internal sealed class AlertBlockParser : BlockParser
    {
        public AlertBlockParser()
        {
            OpeningCharacters = new[] { '!' };
        }

        public override BlockState TryOpen(BlockProcessor processor)
        {
            if (processor.IsCodeIndent)
            {
                return BlockState.None;
            }

            var column = processor.Column;
            var sourcePosition = processor.Start;

            var i = 0;
            while (processor.CurrentChar == '!')
            {
                i++;
                processor.NextChar();
            }

            if(processor.CurrentChar != ' ')
            {
                processor.GoToColumn(column);
                return BlockState.None;
            }

            processor.NextChar();
            if (processor.CurrentChar.IsSpaceOrTab())
            {
                processor.NextColumn();
            }
            

            var type = i == 1 ? AlertBlockType.Information : AlertBlockType.Warning;

            processor.NewBlocks.Push(new AlertBlock(this)
            {
                Level = i,
                Type = type,
                Column = column,
                Span = new SourceSpan(sourcePosition, processor.Line.End),
            });
            return BlockState.Continue;
        }

        public override BlockState TryContinue(BlockProcessor processor, Block block)
        {
            if (processor.IsCodeIndent)
            {
                return BlockState.None;
            }

            var alert = (AlertBlock)block;

            var i = 0;
            while (processor.CurrentChar == '!')
            {
                i++;
                processor.NextChar();
            }

            if (i != alert.Level)
            {
                return processor.IsBlankLine ? BlockState.BreakDiscard : BlockState.None;
            }
            
            var c = processor.NextChar();
            if (c.IsSpace())
            {
                processor.NextChar(); 
            }

            block.UpdateSpanEnd(processor.Line.End);
            return BlockState.Continue;
        }
    }
}