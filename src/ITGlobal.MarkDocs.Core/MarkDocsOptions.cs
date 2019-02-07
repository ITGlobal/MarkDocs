using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Extensions;
using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Impl;
using ITGlobal.MarkDocs.Source;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using ITGlobal.MarkDocs.Source.Impl;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     MarkDocs cache configuration builder
    /// </summary>
    [PublicAPI]
    public sealed class MarkDocsOptions
    {
        private readonly IServiceCollection _services;

        internal MarkDocsOptions(IServiceCollection services)
        {
            _services = services;
        }

        #region Source tree

        private Func<IServiceProvider, ISourceTreeProvider> _sourceTreeProviderFactory;

        /// <summary>
        ///     Configures source tree provider
        /// </summary>
        [NotNull]
        public MarkDocsOptions UseSourceTree([NotNull] Func<IServiceProvider, ISourceTreeProvider> factory)
        {
            _sourceTreeProviderFactory = factory;
            return this;
        }

        internal Action<IServiceCollection> ConfigureCustomAssetTreeReader { get; set; }

        #endregion

        #region ResourceUrlResolver

        private Func<IServiceProvider, IResourceUrlResolver> _resourceUrlResolverFactory;

        /// <summary>
        ///     Sets resource URL resolver implementation
        /// </summary>
        [NotNull]
        public MarkDocsOptions UseResourceUrlResolver([NotNull] Func<IServiceProvider, IResourceUrlResolver> factory)
        {
            _resourceUrlResolverFactory = factory;
            return this;
        }

        #endregion

        #region Format

        private Func<IServiceProvider, IFormat> _formatFactory;

        /// <summary>
        ///     Sets source format service implementation
        /// </summary>
        [NotNull]
        public MarkDocsOptions UseFormat([NotNull] Func<IServiceProvider, IFormat> factory)
        {
            _formatFactory = factory;
            return this;
        }

        #endregion

        #region Cache

        private Func<IServiceProvider, ICacheProvider> _cacheFactory;

        /// <summary>
        ///     Sets cache service implementation
        /// </summary>
        [NotNull]
        public MarkDocsOptions UseCache([NotNull] Func<IServiceProvider, ICacheProvider> factory)
        {
            _cacheFactory = factory;
            return this;
        }

        #endregion

        #region Extensions

        private readonly List<Func<IServiceProvider, IExtensionFactory>> _extensionFactories
            = new List<Func<IServiceProvider, IExtensionFactory>>();

        /// <summary>
        ///     Adds an extension
        /// </summary>
        [NotNull]
        public MarkDocsOptions AddExtension([NotNull] Func<IServiceProvider, IExtensionFactory> factory)
        {
            _extensionFactories.Add(factory);
            return this;
        }

        #endregion

        #region Callback

        private Func<IServiceProvider, IMarkDocsEventCallback> _callbackFactory = _ => new MarkDocsEventCallback();

        /// <summary>
        ///     Sets a lifetime event callback
        /// </summary>
        [NotNull]
        public MarkDocsOptions UseCallback([NotNull] Func<IServiceProvider, IMarkDocsEventCallback> callbackFactory)
        {
            _callbackFactory = callbackFactory;
            return this;
        }

        #endregion

        #region Logger

        private Func<IServiceProvider, IMarkDocsLog> _logFactory = _ => new NullMarkDocsLog();

        /// <summary>
        ///     Sets a logger implementation
        /// </summary>
        [NotNull]
        public MarkDocsOptions UseLog([NotNull] Func<IServiceProvider, IMarkDocsLog> logFactory)
        {
            _logFactory = logFactory;
            return this;
        }

        #endregion

        /// <summary>
        ///     Applies specified configuration functions
        /// </summary>
        [NotNull]
        internal MarkDocsOptions ConfigureServices([NotNull] Action<IServiceCollection> action)
        {
            action(_services);
            return this;
        }

        internal void Configure()
        {
            if (_sourceTreeProviderFactory == null)
            {
                throw new Exception("No source tree provider has been configured");
            }

            if (_resourceUrlResolverFactory == null)
            {
                throw new Exception("No resource URL resolver has been configured");
            }

            if (_formatFactory == null)
            {
                throw new Exception("No source format service has been configured");
            }

            if (_cacheFactory == null)
            {
                throw new Exception("No cache service has been configured");
            }

            _services.AddSingleton<IMarkDocService, MarkDocService>();
            if (ConfigureCustomAssetTreeReader!= null)
            {
                ConfigureCustomAssetTreeReader(_services);
            }
            else
            {
                _services.AddSingleton<IAssetTreeReader, AssetTreeReader>();
            }

            _services.TryAddSingleton<IContentHashProvider, Sha1ContentHashProvider>();
            _services.TryAddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>();
            _services.AddSingleton<IContentMetadataProvider, DefaultContentMetadataProvider>();

            _services.AddSingleton(_sourceTreeProviderFactory);
            _services.AddSingleton(_resourceUrlResolverFactory);
            _services.AddSingleton(_formatFactory);
            _services.AddSingleton(_cacheFactory);

            _services.AddSingleton(_callbackFactory);
            _services.AddSingleton(_logFactory);
            _services.AddSingleton(CreateExtensionCollection);

            ExtensionCollection CreateExtensionCollection(IServiceProvider serviceProvider)
            {
                return new ExtensionCollection(CreateExtensionFactories());

                IEnumerable<IExtensionFactory> CreateExtensionFactories()
                {
                    foreach (var factory in _extensionFactories)
                    {
                        yield return factory(serviceProvider);
                    }
                }
            }
        }
    }
}