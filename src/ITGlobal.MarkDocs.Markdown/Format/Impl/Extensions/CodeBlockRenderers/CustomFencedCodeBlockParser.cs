using Markdig.Parsers;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.CodeBlockRenderers
{
    internal sealed class CustomFencedCodeBlockParser : FencedCodeBlockParser
    {
        private readonly CodeBlockRendererSelector _selector;

        public CustomFencedCodeBlockParser(CodeBlockRendererSelector selector)
        {
            _selector = selector;
        }

        public override bool Close(BlockProcessor processor, Block block)
        {
            if (!base.Close(processor, block))
            {
                return false;
            }

            if (!MarkdownPageReadContext.IsPresent)
            {
                return true;
            }

            var context = MarkdownPageReadContext.Current;
            
            try
            {
                var codeBlock = (FencedCodeBlock)block;
                var renderable = _selector.TryCreateRenderable(context, codeBlock);
                if (renderable != null)
                {
                    block.SetCustomRenderable(renderable);
                }
            }
            catch
            {
                context.Error("Error while rendering code block", block.Line);
            }

            return true;
        }
    }
}