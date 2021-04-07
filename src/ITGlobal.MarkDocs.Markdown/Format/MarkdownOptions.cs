using ITGlobal.MarkDocs.Format.Impl.Metadata;
using JetBrains.Annotations;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using ITGlobal.MarkDocs.Format.Impl;
using ITGlobal.MarkDocs.Format.Impl.Extensions.ChildrenList;
using ITGlobal.MarkDocs.Format.Impl.Extensions.CustomBlockRendering;
using ITGlobal.MarkDocs.Format.Impl.Extensions.LaTeX;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     Options for markdown format
    /// </summary>
    [PublicAPI]
    public sealed partial class MarkdownOptions
    {

        private readonly List<Action<IServiceCollection>> _customRegistrations
            = new List<Action<IServiceCollection>>();

        internal MarkdownOptions Register(Action<IServiceCollection> action)
        {
            _customRegistrations.Add(action);
            return this;
        }

        /// <summary>
        ///     .ctor
        /// </summary>
        public MarkdownOptions()
        {
            CodeBlocks = new MarkdownCodeBlockRenderingOptions(this);
            Images = new MarkdownImageRenderingOptions(this);
            HtmlBlocks = new MarkdownHtmlBlockRenderingOptions(this);
        }

        #region Code Blocks

        /// <summary>
        ///     Code block rendering options
        /// </summary>
        [NotNull]
        public MarkdownCodeBlockRenderingOptions CodeBlocks { get; }

        #endregion

        #region Images

        /// <summary>
        ///     Image rendering options
        /// </summary>
        [NotNull]
        public MarkdownImageRenderingOptions Images { get; }

        #endregion

        #region HTML Blocks

        /// <summary>
        ///     HTML block rendering options
        /// </summary>
        [NotNull]
        public MarkdownHtmlBlockRenderingOptions HtmlBlocks { get; }

        #endregion

        #region MathRenderer

        private Func<IServiceProvider, IMathRenderer> _mathRendererFactory
            = _ => new CodecogsMathRenderer(CodecogsMathRenderer.DefaultUrl);

        /// <summary>
        ///     Sets MathML/Tex/LaTex renderer implementation
        /// </summary>
        [NotNull]
        public MarkdownOptions UseMathRenderer([NotNull] Func<IServiceProvider, IMathRenderer> factory)
        {
            _mathRendererFactory = factory;
            return this;
        }

        /// <summary>
        ///     Sets MathML/Tex/LaTex renderer implementation
        /// </summary>
        [NotNull]
        public MarkdownOptions UseMathRenderer([NotNull] IMathRenderer renderer)
            => UseMathRenderer(_ => renderer);

        /// <summary>
        ///     Sets MathML/Tex/LaTex renderer implementation
        /// </summary>
        [NotNull]
        public MarkdownOptions UseMathRenderer<T>()
            where T : class, IMathRenderer
            => Register(_ => _.AddSingleton<T>()).UseMathRenderer(_ => _.GetRequiredService<T>());

        #endregion

        #region TocRenderer

        private Func<IServiceProvider, ITocRenderer> _tocRendererFactory;

        /// <summary>
        ///     Sets table-of-contents renderer implementation
        /// </summary>
        [NotNull]
        public MarkdownOptions UseTocRenderer([NotNull] Func<IServiceProvider, ITocRenderer> factory)
        {
            _tocRendererFactory = factory;
            return this;
        }

        /// <summary>
        ///     Sets table-of-contents renderer implementation
        /// </summary>
        [NotNull]
        public MarkdownOptions UseTocRenderer([NotNull] ITocRenderer renderer)
            => UseTocRenderer(_ => renderer);

        /// <summary>
        ///     Sets table-of-contents renderer implementation
        /// </summary>
        [NotNull]
        public MarkdownOptions UseTocRenderer<T>()
            where T : class, ITocRenderer
            => Register(_ => _.AddSingleton<T>()).UseTocRenderer(_ => _.GetRequiredService<T>());

        #endregion

        #region ChildrenListRenderer

        private Func<IServiceProvider, IChildrenListRenderer> _childrenListRendererFactory
            = _ => _.GetRequiredService<DefaultChildrenListRenderer>();

        /// <summary>
        ///     Sets children-list renderer implementation
        /// </summary>
        [NotNull]
        public MarkdownOptions UseChildrenListRenderer([NotNull] Func<IServiceProvider, IChildrenListRenderer> factory)
        {
            _childrenListRendererFactory = factory;
            return this;
        }

        /// <summary>
        ///     Sets children-list renderer implementation
        /// </summary>
        [NotNull]
        public MarkdownOptions UseChildrenListRenderer([NotNull] IChildrenListRenderer renderer)
            => UseChildrenListRenderer(_ => renderer);

        /// <summary>
        ///     Sets children-list renderer implementation
        /// </summary>
        [NotNull]
        public MarkdownOptions UseChildrenListRenderer<T>()
            where T : class, IChildrenListRenderer
            => Register(_ => _.AddSingleton<T>()).UseChildrenListRenderer(_ => _.GetRequiredService<T>());

        #endregion

        #region OverrideRendering

        private readonly List<Action<IServiceProvider, HtmlRenderer>> _htmlRendererOverrides =
            new List<Action<IServiceProvider, HtmlRenderer>>();

        /// <summary>
        ///     Sets custom rendering function for <typeparamref name="T"/>
        /// </summary>
        [NotNull]
        public MarkdownOptions OverrideRendering<T>([NotNull] Action<HtmlRenderer, T> func)
            where T : MarkdownObject
        {
            OverrideRendering<T>(
                (renderer, block) =>
                {
                    func(renderer, block);
                    return true;
                }
            );
            return this;
        }

        /// <summary>
        ///     Sets custom rendering function for <typeparamref name="T"/>
        /// </summary>
        [NotNull]
        public MarkdownOptions OverrideRendering<T>([NotNull] Func<IServiceProvider, HtmlRenderer, T, bool> func)
            where T : MarkdownObject
        {
            _htmlRendererOverrides.Add(
                (serviceProvider, htmlRenderer) =>
                {
                    if (htmlRenderer.ObjectRenderers.FirstOrDefault(_ => _ is HtmlObjectRenderer<T>) is
                        HtmlObjectRenderer<T> existing)
                    {
                        existing.TryWriters.Add((renderer, block) => func(serviceProvider, renderer, block));
                    }
                    else
                    {
                        htmlRenderer.ObjectRenderers.Add(
                            new HtmlObjectRendererAdapter<T>(
                                (h, block) => func(serviceProvider, h, block)
                            )
                        );
                    }
                }
            );

            return this;
        }

        /// <summary>
        ///     Sets custom rendering function for <typeparamref name="T"/>
        /// </summary>
        [NotNull]
        public MarkdownOptions OverrideRendering<T>([NotNull] Func<HtmlRenderer, T, bool> func)
            where T : MarkdownObject
        {
            return OverrideRendering<T>((_, htmlRenderer, block) => func(htmlRenderer, block));
        }

        #endregion

        #region Service registration

        internal void RegisterServices(IServiceCollection services)
        {
            foreach (var action in _customRegistrations)
            {
                action(services);
            }

            if (_mathRendererFactory != null)
            {
                services.AddSingleton(_mathRendererFactory);
            }

            if (_tocRendererFactory != null)
            {
                services.AddSingleton(_tocRendererFactory);
            }

            if (_childrenListRendererFactory != null)
            {
                services.AddSingleton(_childrenListRendererFactory);
            }

            services.AddSingleton(this);
            services.AddSingleton<MarkdownPipelineFactory>();
            services.AddSingleton<IMetadataExtractor, DefaultMetadataExtractor>();

            CodeBlocks.RegisterServices(services);
            Images.RegisterServices(services);
            HtmlBlocks.RegisterServices(services);

            services.AddSingleton<DefaultChildrenListRenderer>();
        }

        #endregion

    }
}