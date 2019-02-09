using System;
using System.Collections.Generic;
using System.Linq;
using ITGlobal.MarkDocs.Format.Impl.Extensions.Images;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     Configures image rendering
    /// </summary>
    [PublicAPI]
    public sealed class MarkdownImageRenderingOptions
    {
        private readonly List<Action<IServiceCollection>> _customRegistrations
            = new List<Action<IServiceCollection>>();

        private readonly List<Func<IServiceProvider, IImageRenderer>> _defaultImplementations
            = new List<Func<IServiceProvider, IImageRenderer>>();

        internal MarkdownImageRenderingOptions(MarkdownOptions options)
        {
            RootOptions = options;
        }

        internal MarkdownOptions RootOptions { get; }

        /// <summary>
        ///     Adds an image renderer implementation
        /// </summary>
        [NotNull]
        public MarkdownOptions Use([NotNull] Func<IServiceProvider, IImageRenderer> factory)
        {
            _defaultImplementations.Add(factory);
            return RootOptions;
        }

        /// <summary>
        ///     Adds an image renderer implementation
        /// </summary>
        [NotNull]
        public MarkdownOptions Use<T>()
            where T : class, IImageRenderer
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

        private ImageRendererSelector CreateSelector(IServiceProvider services)
        {
            var defaultRendererChain = _defaultImplementations
                .Select(f => f(services))
                .ToArray();

            return new ImageRendererSelector(defaultRendererChain);
        }
    }
}