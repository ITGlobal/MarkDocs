using Markdig;
using Markdig.Extensions.Tables;
using Microsoft.Extensions.DependencyInjection;
using System;
using ITGlobal.MarkDocs.Format.Impl.Extensions.Admonition;
using ITGlobal.MarkDocs.Format.Impl.Extensions.Alerts;
using ITGlobal.MarkDocs.Format.Impl.Extensions.ChildrenList;
using ITGlobal.MarkDocs.Format.Impl.Extensions.CodeBlockRenderers;
using ITGlobal.MarkDocs.Format.Impl.Extensions.CustomBlockRendering;
using ITGlobal.MarkDocs.Format.Impl.Extensions.CustomHeading;
using ITGlobal.MarkDocs.Format.Impl.Extensions.Cut;
using ITGlobal.MarkDocs.Format.Impl.Extensions.Icons;
using ITGlobal.MarkDocs.Format.Impl.Extensions.Mathematics;
using ITGlobal.MarkDocs.Format.Impl.Extensions.TableOfContents;

namespace ITGlobal.MarkDocs.Format
{
    partial class MarkdownOptions
    {
        internal MarkdownPipeline CreateMarkdownPipeline(IServiceProvider serviceProvider)
        {
            var builder = new MarkdownPipelineBuilder();

            if (_useAbbreviations)
            {
                builder.UseAbbreviations();
            }
            if (_useAutoIdentifiers)
            {
                builder.UseAutoIdentifiers();
            }
            if (_useCitations)
            {
                builder.UseCitations();
            }
            if (_useCustomContainers)
            {
                builder.UseCustomContainers();
            }
            if (_useDefinitionLists)
            {
                builder.UseDefinitionLists();
            }
            if (_useEmphasisExtras)
            {
                builder.UseEmphasisExtras();
            }
            if (_useGridTables)
            {
                builder.UseGridTables();
            }
            if (_useHtmlAttributes)
            {
                builder.UseGenericAttributes();
            }
            if (_useFigures)
            {
                builder.UseFigures();
            }

            if (_useFooters)
            {
                builder.UseFooters();
            }
            if (_useFootnotes)
            {
                builder.UseFootnotes();
            }
            if (_useMediaLinks)
            {
                builder.UseMediaLinks();
            }
            if (_usePipeTables)
            {
                builder.UsePipeTables(new PipeTableOptions { RequireHeaderSeparator = true });
            }
            if (_useListExtras)
            {
                builder.UseListExtras();
            }
            if (_useTaskLists)
            {
                builder.UseTaskLists();
            }
            if (_useBootstrap)
            {
                builder.UseBootstrap();
            }
            if (_useEmojiAndSmiley)
            {
                builder.UseEmojiAndSmiley();
            }
            if (_useSmartyPants)
            {
                builder.UseSmartyPants();
            }

            if (_useIcons)
            {
                builder.Extensions.AddIfNotAlready<IconExtension>();
            }

            builder.UseYamlFrontMatter();
            if (_jiraLinkOptions != null)
            {
                builder.UseJiraLinks(_jiraLinkOptions);
            }

            var tocRenderer = serviceProvider.GetService<ITocRenderer>();
            if (tocRenderer != null)
            {
                builder.Extensions.AddIfNotAlready(new TableOfContentsExtension(tocRenderer));
            }

            var childrenListRenderer = serviceProvider.GetService<IChildrenListRenderer>();
            if (childrenListRenderer != null)
            {
                builder.Extensions.Add(new ChildrenListExtension(childrenListRenderer));
            }

            var mathRenderer = serviceProvider.GetService<IMathRenderer>();
            if (mathRenderer != null)
            {
                builder.Extensions.Add(new MathematicsExtension(mathRenderer));
            }

            var codeBlockRendererSelector = serviceProvider.GetService<CodeBlockRendererSelector>();
            if (codeBlockRendererSelector != null)
            {
                builder.Extensions.Add(new CustomCodeBlockRenderingExtension(codeBlockRendererSelector));
            }

            builder.Extensions.AddIfNotAlready(new CustomHeadingExtension(_dontRenderFirstHeading));

            if (_useAdmonition)
            {
                builder.Extensions.Add(new AdmonitionExtension());
            }

            if (_useAlerts)
            {
                builder.Extensions.Add(new AlertBlockExtension());
            }

            builder.Extensions.Add(new CutBlockExtension());

            if (_htmlRendererOverrides.Count > 0)
            {
                builder.Extensions.Add(new CustomBlockRenderingExtension(_htmlRendererOverrides));
            }

            return builder.Build();
        }
    }
}
