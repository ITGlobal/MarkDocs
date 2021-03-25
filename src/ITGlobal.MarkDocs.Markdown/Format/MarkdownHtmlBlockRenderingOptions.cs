using System;
using System.Collections.Generic;
using System.Linq;
using ITGlobal.MarkDocs.Format.Impl.Extensions.Html;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     Configures HTML block rendering
    /// </summary>
    [PublicAPI]
    public sealed class MarkdownHtmlBlockRenderingOptions
    {
        private readonly List<Action<IServiceCollection>> _customRegistrations
            = new List<Action<IServiceCollection>>();

        private readonly List<Func<IServiceProvider, IHtmlBlockRenderer>> _defaultImplementations
            = new List<Func<IServiceProvider, IHtmlBlockRenderer>>();

        internal MarkdownHtmlBlockRenderingOptions(MarkdownOptions options)
        {
            RootOptions = options;
        }

        internal MarkdownOptions RootOptions { get; }

        /// <summary>
        ///     Adds an HTML block renderer implementation
        /// </summary>
        [NotNull]
        public MarkdownOptions Use([NotNull] Func<IServiceProvider, IHtmlBlockRenderer> factory)
        {
            _defaultImplementations.Add(factory);
            return RootOptions;
        }

        /// <summary>
        ///     Adds an HTML block renderer implementation
        /// </summary>
        [NotNull]
        public MarkdownOptions Use<T>()
            where T : class, IHtmlBlockRenderer
        {
            _customRegistrations.Add(_ => _.AddSingleton<T>());
            _defaultImplementations.Add(_ => _.GetRequiredService<T>());
            return RootOptions;
        }

        internal void RegisterServices(IServiceCollection services)
        {
            foreach (var action in _customRegistrations)
            {
                action(services);
            }

            services.AddSingleton(CreateSelector);
        }

        private void Register(Action<IServiceCollection> action)
        {
            _customRegistrations.Add(action);
        }

        private HtmlBlockRendererSelector CreateSelector(IServiceProvider services)
        {
            var defaultRendererChain = _defaultImplementations
                .Select(f => f(services))
                .ToArray();

            return new HtmlBlockRendererSelector(defaultRendererChain);
        }
    }
}