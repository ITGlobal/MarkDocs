using ITGlobal.MarkDocs.Format.ChildrenList;
using ITGlobal.MarkDocs.Format.CustomHeading;
using ITGlobal.MarkDocs.Format.Icons;
using ITGlobal.MarkDocs.Format.TableOfContents;
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

        public static MarkdownPipelineBuilder UseChildrenList(
            this MarkdownPipelineBuilder pipeline,
            IResourceUrlResolver resourceUrlResolver)
        {
            pipeline.Extensions.Add(new ChildrenListExtension(resourceUrlResolver));
            return pipeline;
        }

        public static MarkdownPipelineBuilder UseCustomHeading(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<CustomHeadingExtension>();
            return pipeline;
        }
    }
}