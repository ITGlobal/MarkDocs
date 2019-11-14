using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Markdig.Syntax.Inlines;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.Images
{
    internal sealed class ImageRendererSelector
    {
        private readonly List<IImageRenderer> _defaultRendererChain;

        public ImageRendererSelector(IImageRenderer[] defaultRendererChain)
        {
            _defaultRendererChain = defaultRendererChain.ToList();
        }

        public void Register(IImageRenderer renderer)
        {
            _defaultRendererChain.Add(renderer);
        }

        [CanBeNull]
        public IRenderable TryCreateRenderable([NotNull] IPageReadContext ctx, [NotNull] LinkInline inline)
        {
            foreach (var renderer in _defaultRendererChain)
            {
                var renderable = renderer.TryCreateRenderable(ctx, inline);
                if (renderable != null)
                {
                    return renderable;
                }
            }

            return null;
        }
    }
}