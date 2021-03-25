using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.Html
{
    internal sealed class HtmlBlockRendererSelector
    {
        private readonly List<IHtmlBlockRenderer> _defaultRendererChain;

        public HtmlBlockRendererSelector(IHtmlBlockRenderer[] defaultRendererChain)
        {
            _defaultRendererChain = defaultRendererChain.ToList();
        }

        public void Register(IHtmlBlockRenderer renderer)
        {
            _defaultRendererChain.Add(renderer);
        }

        [CanBeNull]
        public IRenderable TryCreateRenderable([NotNull] IPageReadContext ctx, [NotNull] ParsedHtmlBlock block)
        {
            foreach (var renderer in _defaultRendererChain)
            {
                var renderable = renderer.TryCreateRenderable(ctx, block);
                if (renderable != null)
                {
                    return renderable;
                }
            }

            return null;
        }
    }
}
