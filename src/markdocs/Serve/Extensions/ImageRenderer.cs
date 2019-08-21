using System.Collections.Generic;
using System.Linq;
using ITGlobal.MarkDocs.Tools.Serve.Models;
using Markdig.Renderers;
using Markdig.Syntax.Inlines;

namespace ITGlobal.MarkDocs.Tools.Serve.Extensions
{
    public static class ImageRenderer
    {
        public static bool RenderImage(HtmlRenderer renderer, LinkInline link)
        {
            if (!link.IsImage)
            {
                return false;
            }

            CollectSiblings(link, out var sublings, out var areSublingsImages, out var isLastOne);

            if (!areSublingsImages)
            {
                return false;
            }

            if (sublings.Count < 2)
            {
                RenderOneImage(renderer, link);
                return true;
            }

            if (!isLastOne)
            {
                return true;
            }

            RenderImageGallery(renderer, sublings);
            return true;
        }

        private static void CollectSiblings(
            LinkInline link,
            out List<LinkInline> sublings,
            out bool areSublingsImages,
            out bool isLastOne)
        {
            sublings = new List<LinkInline>();
            isLastOne = true;
            areSublingsImages = true;

            var s = link.PreviousSibling;
            while (s != null)
            {
                switch (s)
                {
                    case LinkInline prevLink:
                        if (!prevLink.IsImage)
                        {
                            areSublingsImages = false;
                            return;
                        }

                        sublings.Add(prevLink);
                        break;

                    case LineBreakInline _:
                        break;

                    default:
                        areSublingsImages = false;
                        return;
                }

                s = s.PreviousSibling;
            }

            sublings.Add(link);

            s = link.NextSibling;
            while (s != null)
            {
                switch (s)
                {
                    case LinkInline nextLink:
                        if (!nextLink.IsImage)
                        {
                            areSublingsImages = false;
                            return;
                        }

                        isLastOne = false;
                        sublings.Add(nextLink);
                        break;

                    case LineBreakInline _:
                        break;

                    default:
                        areSublingsImages = false;
                        return;
                }

                s = s.NextSibling;
            }
        }

        private static void RenderOneImage(HtmlRenderer renderer, LinkInline link)
        {
            var model = GetImageModel(link);
            var html = RazorViewRenderer.Render(Startup.ApplicationServices, "~/Serve/Views/Render/Image.cshtml", model);
            renderer.Write(html);
        }

        private static void RenderImageGallery(HtmlRenderer renderer, List<LinkInline> sublings)
        {
            var model = new ImageGalleryModel(sublings.Select(GetImageModel).ToArray());
            var html = RazorViewRenderer.Render(Startup.ApplicationServices, "~/Serve/Views/Render/ImageGallery.cshtml", model);
            renderer.Write(html);
        }

        private static ImageModel GetImageModel(LinkInline link)
        {
            var url = link.GetDynamicUrl != null ? link.GetDynamicUrl() ?? link.Url : link.Url;
            var model = new ImageModel(url, link.Title, link.Title);
            return model;
        }
    }
}