using System;
using System.Collections.Generic;
using System.Linq;
using ITGlobal.MarkDocs.Format.Impl.Extensions.CodeBlockRenderers;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     Configures code block rendering
    /// </summary>
    [PublicAPI]
    public sealed class MarkdownCodeBlockRenderingOptions
    {
        private readonly List<Action<IServiceCollection>> _customRegistrations
            = new List<Action<IServiceCollection>>();

        private readonly List<Func<IServiceProvider, ICodeBlockRenderer>> _defaultImplementations
            = new List<Func<IServiceProvider, ICodeBlockRenderer>>();
        private readonly Dictionary<string, List<Func<IServiceProvider, ICodeBlockRenderer>>> _specificImplementations
            = new Dictionary<string, List<Func<IServiceProvider, ICodeBlockRenderer>>>(StringComparer.OrdinalIgnoreCase);

        internal MarkdownCodeBlockRenderingOptions(MarkdownOptions options)
        {
            RootOptions = options;
        }

        internal MarkdownOptions RootOptions { get; }

        /// <summary>
        ///     Adds a code block renderer implementation into chain
        /// </summary>
        [NotNull]
        public MarkdownOptions Use([NotNull] Func<IServiceProvider, ICodeBlockRenderer> factory)
        {
            _defaultImplementations.Add(factory);
            return RootOptions;
        }

        /// <summary>
        ///     Adds a code block renderer implementation into chain
        /// </summary>
        [NotNull]
        public MarkdownOptions Use<T>()
            where T : class, ICodeBlockRenderer
        {
            _customRegistrations.Add(_ => _.AddSingleton<T>());
            _defaultImplementations.Add(_ => _.GetRequiredService<T>());
            return RootOptions;
        }

        /// <summary>
        ///     Adds a code block renderer implementation into chain
        /// </summary>
        [NotNull]
        public MarkdownOptions Use([NotNull] string lang, [NotNull] Func<IServiceProvider, ICodeBlockRenderer> factory)
        {
            if (!_specificImplementations.TryGetValue(lang, out var list))
            {
                list = new List<Func<IServiceProvider, ICodeBlockRenderer>>();
                _specificImplementations.Add(lang, list);
            }
            list.Add(factory);
            return RootOptions;
        }

        /// <summary>
        ///     Adds a code block renderer implementation into chain
        /// </summary>
        [NotNull]
        public MarkdownOptions Use<T>([NotNull] string lang)
            where T : class, ICodeBlockRenderer
        {
            _customRegistrations.Add(x => x.AddSingleton<T>());
            if (!_specificImplementations.TryGetValue(lang, out var list))
            {
                list = new List<Func<IServiceProvider, ICodeBlockRenderer>>();
                _specificImplementations.Add(lang, list);
            }
            list.Add(_ => _.GetRequiredService<T>());
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

        private CodeBlockRendererSelector CreateSelector(IServiceProvider services)
        {
            var defaultRendererChain = _defaultImplementations
                .Select(f => f(services))
                .ToArray();

            var specificRendererChain = _specificImplementations
                .ToDictionary(
                    _ => _.Key,
                    _ => _.Value.Select(f => f(services)).ToArray(),
                    StringComparer.OrdinalIgnoreCase
                );

            return new CodeBlockRendererSelector(
                defaultRendererChain,
                specificRendererChain
            );
        }
    }
}