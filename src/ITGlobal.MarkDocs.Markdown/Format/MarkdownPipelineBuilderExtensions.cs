using ITGlobal.MarkDocs.Format.ChildrenList;
using ITGlobal.MarkDocs.Format.CodeBlockRenderers;
using ITGlobal.MarkDocs.Format.CustomHeading;
using ITGlobal.MarkDocs.Format.Icons;
using ITGlobal.MarkDocs.Format.TableOfContents;
using ITGlobal.MarkDocs.Markdown.Format;
using ITGlobal.MarkDocs.Markdown.Format.Admonition;
using ITGlobal.MarkDocs.Markdown.Format.Cut;
using Markdig;

namespace ITGlobal.MarkDocs.Format
{
    internal static class MarkdownPipelineBuilderExtensions
    {
        public static MarkdownPipelineBuilder UseIcons(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<IconExtension>();
            return pipeline;
        }

        public static MarkdownPipelineBuilder UseTableOfContents(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<TableOfContentsExtension>();
            return pipeline;
        }

        public static MarkdownPipelineBuilder UseChildrenList(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.Add(new ChildrenListExtension());
            return pipeline;
        }

        public static MarkdownPipelineBuilder UseCustomHeading(this MarkdownPipelineBuilder pipeline, bool dontRenderFirstHeading)
        {
            pipeline.Extensions.AddIfNotAlready(new CustomHeadingExtension(dontRenderFirstHeading));
            return pipeline;
        }

        public static MarkdownPipelineBuilder UseCustomCodeBlockRendering(
            this MarkdownPipelineBuilder pipeline,
            MarkdownOptions options)
        {
            pipeline.Extensions.Add(new CustomCodeBlockRenderingExtension(options));
            return pipeline;
        }

        public static MarkdownPipelineBuilder UseAdmonitions(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.Add(new AdmonitionExtension());
            return pipeline;
        }

        public static MarkdownPipelineBuilder UseAlerts(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.Add(new AlertBlockExtension());
            return pipeline;
        }

        public static MarkdownPipelineBuilder UseCuts(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.Add(new CutBlockExtension());
            return pipeline;
        }
    }
}