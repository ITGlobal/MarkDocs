using ITGlobal.MarkDocs.Format.Impl.Extensions.CodeBlockRenderers;
using ITGlobal.MarkDocs.Format.Impl.Extensions.HighlightJs;
using ITGlobal.MarkDocs.Format.Impl.Extensions.Images;
using ITGlobal.MarkDocs.Format.Impl.Extensions.LaTeX;
using ITGlobal.MarkDocs.Format.Impl.Extensions.PlantUml;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     Extension methods for <see cref="MarkdownOptionsExtensions"/>
    /// </summary>
    [PublicAPI]
    public static class MarkdownOptionsExtensions
    {
        #region Mermaid diagrams

        /// <summary>
        ///     Adds support for "mermaid" syntax (with client-side rendering)
        /// </summary>
        [NotNull]
        public static MarkdownOptions AddMermaidDiagrams(this MarkdownOptions options)
        {
            options.CodeBlocks.Use<MermaidRenderer>(MermaidRenderer.Language);
            return options;
        }

        #endregion

        #region PlantUML

        /// <summary>
        ///     Adds support for "plantuml" syntax (with server-side rendering)
        /// </summary>
        [NotNull]
        public static MarkdownOptions AddPlantUmlWebService(this MarkdownOptions options, string url)
        {
            options.Register(_ => _.AddSingleton<PlantUmlRenderer>(new PlantUmlWebServiceRenderer(url)));
            options.Register(_ => _.AddSingleton<PlantUmlCodeBlockRenderer>());
            options.Register(_ => _.AddSingleton<PlantUmlImageRenderer>());
            options.CodeBlocks.Use(PlantUmlRenderer.Language, _ => _.GetRequiredService<PlantUmlCodeBlockRenderer>());
            options.Images.Use(_ => _.GetRequiredService<PlantUmlImageRenderer>());
            return options;
        }

        /// <summary>
        ///     Adds support for "plantuml" syntax (with server-side rendering)
        /// </summary>
        [NotNull]
        public static MarkdownOptions AddPlantUmlWebService(this MarkdownOptions options)
        {
            return options.AddPlantUmlWebService(PlantUmlWebServiceRenderer.DefaultUrl);
        }

        #endregion

        #region LaTeX

        /// <summary>
        ///     Adds support for "latex" syntax (with server-side rendering)
        /// </summary>
        [NotNull]
        public static MarkdownOptions AddCodecogsMathRenderer(this MarkdownOptions options, string url)
        {
            options.UseMathRenderer(_ => new CodecogsMathRenderer(url));
            return options;
        }

        /// <summary>
        ///     Adds support for "latex" syntax (with server-side rendering)
        /// </summary>
        [NotNull]
        public static MarkdownOptions AddCodecogsMathRenderer(this MarkdownOptions options)
        {
            return options.AddCodecogsMathRenderer(CodecogsMathRenderer.DefaultUrl);
        }

        #endregion

        #region Highlight.js (client-side)

        /// <summary>
        ///     Adds code highlight support (with client-side rendering)
        /// </summary>
        [NotNull]
        public static MarkdownOptions AddClientSideHighlightJs(this MarkdownOptions options)
        {
            var renderer = new ClientSideHighlightJsCodeBlockRenderer();
            foreach (var language in ClientSideHighlightJsCodeBlockRenderer.SupportedLanguages)
            {
                options.CodeBlocks.Use(language, _ => renderer);
            }

            return options;
        }

        #endregion

        #region Highlight.js

        /// <summary>
        ///     Adds code highlight support (with server-side rendering)
        /// </summary>
        [NotNull]
        public static MarkdownOptions AddHighlightJs(
            this MarkdownOptions options,
            Action<HighlightJsOptions> configure)
        {
            var opt = new HighlightJsOptions();
            configure(opt);

            options.Register(_ => _.AddSingleton(opt));
            options.Register(_ => _.AddSingleton<HighlightJsWorker>());
            options.Register(_ => _.AddSingleton<ServerSideHighlightJsCodeBlockRenderer>());

            options.CodeBlocks.Use(_ => _.GetRequiredService<ServerSideHighlightJsCodeBlockRenderer>());
            return options;
        }

        /// <summary>
        ///     Adds code highlight support (with server-side rendering)
        /// </summary>
        [NotNull]
        public static MarkdownOptions AddHighlightJs(
            this MarkdownOptions options,
            string tempDirectory,
            HighlightJsStylesheet stylesheet = HighlightJsStylesheet.Vs
        )
        {
            return options.AddHighlightJs(_ =>
            {
                _.TempDirectory = tempDirectory;
                _.Stylesheet = stylesheet;
            });
        }

        #endregion
    }
}
