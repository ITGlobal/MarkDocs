using System;
using ITGlobal.MarkDocs.Format.Impl.Extensions.CodeBlockRenderers;
using ITGlobal.MarkDocs.Format.Impl.Extensions.Mathematics;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace ITGlobal.MarkDocs.Format
{
    [PublicAPI]
    public static class CodeBlockRenderingExtensions
    {
        [NotNull]
        public static MarkdownOptions UseMermaidDiagrams(this MarkdownCodeBlockRenderingOptions options)
        {
            return options.Use<MermaidRenderer>(MermaidRenderer.Language);
        }

        [NotNull]
        public static MarkdownOptions UsePlantUmlWebService(this MarkdownCodeBlockRenderingOptions options, string url)
        {
            return options.Use(PlantUmlRenderer.Language, _ => new PlantUmlWebServiceRenderer(url));
        }

        [NotNull]
        public static MarkdownOptions UsePlantUmlWebService(this MarkdownCodeBlockRenderingOptions options)
        {
            return options.UsePlantUmlWebService(PlantUmlWebServiceRenderer.DefaultUrl);
        }
        
        [NotNull]
        public static MarkdownOptions UseCodecogsMathRenderer(this MarkdownOptions options, string url)
        {
            return options.UseMathRenderer(_ => new CodecogsMathRenderer(url));
        }

        [NotNull]
        public static MarkdownOptions UseCodecogsMathRenderer(this MarkdownOptions options)
        {
            return options.UseCodecogsMathRenderer(CodecogsMathRenderer.DefaultUrl);
        }

        [NotNull]
        public static MarkdownOptions UseClientSideHighlightJs(this MarkdownCodeBlockRenderingOptions options)
        {
            var renderer = new ClientSideHighlightJsCodeBlockRenderer();
            foreach (var language in ClientSideHighlightJsCodeBlockRenderer.SupportedLanguages)
            {
                options.Use(language, _ => renderer);
            }

            return options.RootOptions;
        }

        [NotNull]
        public static MarkdownOptions UseServerSideHighlightJs(
            this MarkdownCodeBlockRenderingOptions options,
            Action<HighlightJsOptions> configure)
        {
            var opt = new HighlightJsOptions();
            configure(opt);

            options.RootOptions.Register(_ => _.AddSingleton(opt));
            options.RootOptions.Register(_ => _.AddSingleton<HighlightJsWorker>());
            options.RootOptions.Register(_ => _.AddSingleton<ServerSideHighlightJsCodeBlockRenderer>());

            return options.Use(_ => _.GetRequiredService<ServerSideHighlightJsCodeBlockRenderer>());
        }

        [NotNull]
        public static MarkdownOptions UseServerSideHighlightJs(
            this MarkdownCodeBlockRenderingOptions options,
            string tempDirectory,
            HighlightJsStylesheet stylesheet = HighlightJsStylesheet.Vs
            )
        {
            return options.UseServerSideHighlightJs(_ =>
            {
                _.TempDirectory = tempDirectory;
                _.Stylesheet = stylesheet;
            });
        }
    }
}
