﻿using System.Linq;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.CustomHeading
{
    internal sealed class CustomHeadingRenderer : HeadingRenderer
    {
        private readonly bool _dontRenderFirstHeading;

        public CustomHeadingRenderer(bool dontRenderFirstHeading)
        {
            _dontRenderFirstHeading = dontRenderFirstHeading;
        }

        private const string DOCUMENT_PROPERTY_KEY = "document";
        private const string NO_RENDER_PROPERTY_KEY = "no-render";

        public static void SetDocument(HeadingBlock block, MarkdownDocument document) => block.SetData(DOCUMENT_PROPERTY_KEY, document);
        public static void DontRender(HeadingBlock block) => block.SetData(NO_RENDER_PROPERTY_KEY, "");

        protected override void Write(HtmlRenderer renderer, HeadingBlock block)
        {
            if (_dontRenderFirstHeading)
            {
                var document = block.GetData(DOCUMENT_PROPERTY_KEY) as MarkdownDocument;

                if (document != null && document
                        .OfType<HeadingBlock>()
                        .Where((b, i) => b == block && i == 0)
                        .FirstOrDefault() != null)
                {
                    return;
                }
            }
            
            if (block.GetData(NO_RENDER_PROPERTY_KEY) != null)
            {
                return;
            }

            base.Write(renderer, block);
        }
    }
}