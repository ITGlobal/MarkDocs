using System.Collections.Generic;
using JetBrains.Annotations;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.CodeBlockRenderers
{
    internal sealed class CodeBlockRendererSelector
    {
        private readonly ICodeBlockRenderer[] _defaultRendererChain;
        private readonly Dictionary<string, ICodeBlockRenderer[]> _specificRendererChain;

        public CodeBlockRendererSelector(
            ICodeBlockRenderer[] defaultRendererChain, 
            Dictionary<string, ICodeBlockRenderer[]> specificRendererChain)
        {
            _defaultRendererChain = defaultRendererChain;
            _specificRendererChain = specificRendererChain;
        }
        
        [CanBeNull]
        public ICodeBlockRenderer TrySelect([NotNull] IPageReadContext ctx, [NotNull] FencedCodeBlock block)
        {
            foreach (var renderer in EnumerateRenderers(block))
            {
                if (renderer.CanRender(ctx, block))
                {
                    return renderer;
                }
            }

            return null;
        }

        private IEnumerable<ICodeBlockRenderer> EnumerateRenderers(FencedCodeBlock block)
        {
            if (!string.IsNullOrEmpty(block.Info))
            {
                if (_specificRendererChain.TryGetValue(block.Info, out var list))
                {
                    foreach (var renderer in list)
                    {
                        yield return renderer;
                    }
                }
            }

            foreach (var renderer in _defaultRendererChain)
            {
                yield return renderer;
            }
        }
    }
}