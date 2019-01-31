using ITGlobal.MarkDocs.Extensions;
using ITGlobal.MarkDocs.Impl;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using System;
using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Format;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     MarkDocs cache configuration builder
    /// </summary>
    [PublicAPI]
    public sealed class MarkDocsOptions
    {
        private readonly IServiceCollection _services;

        private Func<IServiceProvider, IMarkDocsEventCallback> _callbackFactory = _ => new MarkDocsEventCallback();
        private Func<IServiceProvider, IMarkDocsLog> _logFactory = _ => new NullMarkDocsLog();

        internal MarkDocsOptions(IServiceCollection services)
        {
            _services = services;

            SourceTree = new MarkDocsSourceTreeOptions(services);
            Format = new MarkDocsFormatOptions(services);
            Cache = new MarkDocsCacheOptions(services);
            Extensions = new MarkDocsExtensionOptions(services);
        }

        /// <summary>
        ///     Source tree configuration builder
        /// </summary>
        [NotNull]
        public MarkDocsSourceTreeOptions SourceTree { get; }

        /// <summary>
        ///     Page format configuration builder
        /// </summary>
        [NotNull]
        public MarkDocsFormatOptions Format { get; }

        /// <summary>
        ///     Content cache configuration builder
        /// </summary>
        [NotNull]
        public MarkDocsCacheOptions Cache { get; }

        /// <summary>
        ///     Extensions configuration builder
        /// </summary>
        [NotNull]
        public MarkDocsExtensionOptions Extensions { get; }

        /// <summary>
        ///     Sets a lifetime event callback
        /// </summary>
        [NotNull]
        public MarkDocsOptions UseCallback([NotNull] Func<IServiceProvider, IMarkDocsEventCallback> callbackFactory)
        {
            _callbackFactory = callbackFactory;
            return this;
        }

        /// <summary>
        ///     Sets a logger implementation
        /// </summary>
        [NotNull]
        public MarkDocsOptions UseLog([NotNull] Func<IServiceProvider, IMarkDocsLog> logFactory)
        {
            _logFactory = logFactory;
            return this;
        }

        /// <summary>
        ///     Applies specified configuration functions
        /// </summary>
        [NotNull]
        public MarkDocsOptions ConfigureServices([NotNull] Action<IServiceCollection> action)
        {
            action(_services);
            return this;
        }

        internal void Configure()
        {
            SourceTree.Configure();
            Format.Configure();
            Cache.Configure();
            Extensions.Configure();

            _services.AddSingleton(_callbackFactory);
            _services.AddSingleton(_logFactory);
        }
    }
}