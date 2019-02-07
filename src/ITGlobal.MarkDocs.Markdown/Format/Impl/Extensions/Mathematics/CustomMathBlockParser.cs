using Markdig.Extensions.Mathematics;
using Markdig.Parsers;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.Mathematics
{
    internal sealed class CustomMathBlockParser : MathBlockParser
    {
        private readonly IMathRenderer _renderer;

        public CustomMathBlockParser(IMathRenderer renderer)
        {
            _renderer = renderer;
        }

        public override bool Close(BlockProcessor processor, Block block)
        {
            if (!base.Close(processor, block))
            {
                return false;
            }

            try
            {
                var renderable = _renderer.CreateRenderable(MarkdownPageReadContext.Current, (MathBlock)block);
                block.SetCustomRenderable(renderable);
            }
            catch
            {
                MarkdownPageReadContext.Current.Error("Error while rendering math block", block.Line);
            }

            return true;
        }
    }
}