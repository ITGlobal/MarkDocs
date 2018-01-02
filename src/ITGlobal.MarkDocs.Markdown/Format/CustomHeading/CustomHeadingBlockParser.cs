using Markdig.Parsers;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.CustomHeading
{
    internal sealed class CustomHeadingBlockParser : HeadingBlockParser
    {
        public override bool Close(BlockProcessor processor, Block block)
        {
            var result = base.Close(processor, block);
            CustomHeadingRenderer.SetDocument((HeadingBlock) block, processor.Document);
            return result;
        }
    }
}