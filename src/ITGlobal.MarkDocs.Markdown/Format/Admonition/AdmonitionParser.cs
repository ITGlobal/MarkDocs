using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Markdown.Format.Admonition
{
    // https://pythonhosted.org/Markdown/extensions/admonition.html
    internal sealed class AdmonitionParser : BlockParser
    {
        public AdmonitionParser()
        {
            OpeningCharacters = new[] { '!' };
        }

        public override BlockState TryOpen(BlockProcessor state)
        {
            if (state.IsCodeIndent)
            {
                return BlockState.None;
            }

            var line = state.Line;

            for (var i = 0; i < 3; i++)
            {
                if (line.CurrentChar != '!')
                {
                    return BlockState.None;
                }
                line.NextChar();
            }

            if (!line.TrimStart())
            {
                return BlockState.None;
            }

            var block = new AdmonitionBlock(this);

            var sb = StringBuilderCache.Local();
            while (char.IsLetter(line.CurrentChar))
            {
                sb.Append(line.CurrentChar);
                line.NextChar();
            }
            block.Type = sb.ToString();

            line.TrimStart();

            
            if (line.CurrentChar == '\"')
            {
                sb = StringBuilderCache.Local();
                line.NextChar();

                while (line.CurrentChar != '\"')
                {
                    sb.Append(line.CurrentChar);
                    line.NextChar();
                }

                block.Title = sb.ToString();
            }

            var column = state.Column;
            state.UnwindAllIndents();
            block.Column = state.Column;
            block.Span = new SourceSpan(state.Line.Start, state.Line.End);
            state.GoToColumn(column);

            state.NewBlocks.Push(block);
            return BlockState.ContinueDiscard;
        }

        public override BlockState TryContinue(BlockProcessor state, Block block)
        {
            if (state.IsBlankLine || !state.IsCodeIndent)
            {
                return BlockState.BreakDiscard;
            }

            if (state.Indent > 4)
            {
                state.GoToCodeIndent();
            }

            block.UpdateSpanEnd(state.Line.End);

            return BlockState.Continue;
        }

        public override bool Close(BlockProcessor processor, Block block)
        {
            var admonitionBlock = (AdmonitionBlock)block;
            if (admonitionBlock.Lines.Count == 0)
            {
                return false;
            }
            var lineCount = admonitionBlock.Lines.Count;
            for (var i = 0; i < lineCount; i++)
            {
                admonitionBlock.Lines.Lines[i].Slice.TrimStart();
            }
            if (lineCount > 0)
            {
                admonitionBlock.Lines.Lines[lineCount - 1].Slice.TrimEnd();
            }

            return true;
        }
    }
}
