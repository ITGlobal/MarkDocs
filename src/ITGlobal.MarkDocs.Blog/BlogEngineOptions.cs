using ITGlobal.MarkDocs.Blog.Impl;
using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Search;
using ITGlobal.MarkDocs.Source;
using ITGlobal.MarkDocs.Tags;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Configuration builder for Blog Engine
    /// </summary>
    [PublicAPI]
    public sealed class BlogEngineOptions
    {
        private readonly IServiceCollection _services;
        private readonly MarkDocsOptions _builder;
        private readonly string _dataDirectory;
        private readonly MarkdownOptions _markdownOptions = new MarkdownOptions();
        private string _rootUrl = "";

        internal BlogEngineOptions(IServiceCollection services, MarkDocsOptions builder, string dataDirectory)
        {
            _services = services;
            _builder = builder;
            _dataDirectory = dataDirectory;

            _builder.UseDiskCache(Path.Combine(_dataDirectory, "cache"));
            _builder.UseResourceUrlResolver(new ResourceUrlResolver(_rootUrl));
            _markdownOptions.CodeBlocks.UseServerSideHighlightJs(Path.Combine(_dataDirectory, "hljs"));
            _markdownOptions.UseChildrenListRenderer<NoneChildrenListRenderer>();
            _markdownOptions.DontRenderFirstHeading();
            _builder.UseMarkdown(_markdownOptions);
            _builder.AddTags();
            _builder.AddSearch(Path.Combine(_dataDirectory, "search"));
            _builder.ConfigureCustomAssetTreeReader = s => s.AddSingleton<IAssetTreeReader, BlogAssetTreeReader>();

            services.AddSingleton<IBlogEngine, BlogEngineImpl>();
            services.AddSingleton<EventConnector>();
            _builder.AddExtension(_ => _.GetRequiredService<EventConnector>());
        }

        /// <summary>
        ///     Applies specified configuration functions
        /// </summary>
        [NotNull]
        internal BlogEngineOptions ConfigureServices([NotNull] Action<IServiceCollection> action)
        {
            action(_services);
            return this;
        }

        /// <summary>
        ///     Sets blog root URL
        /// </summary>
        [NotNull]
        public BlogEngineOptions UseRootUrl([NotNull] string url)
        {
            _rootUrl = url;
            return this;
        }

        /// <summary>
        ///     Sets specified directory as blog data source
        /// </summary>
        [NotNull]
        public BlogEngineOptions FromSourceDirectory([NotNull] string path, bool enableWatch = true)
        {
            _builder.FromStaticDirectory(path, enableWatch);
            return this;
        }

        /// <summary>
        ///     Configures blog engine to use git as blog data source
        /// </summary>
        [NotNull]
        public BlogEngineOptions FromGit([NotNull] string repositoryUrl)
        {
            return FromGit(options =>
            {
                options.FetchUrl = repositoryUrl;
            });
        }

        /// <summary>
        ///     Configures blog engine to use git as blog data source
        /// </summary>
        [NotNull]
        public BlogEngineOptions FromGit([NotNull] Action<GitOptions> configure)
        {
            _builder.FromGit(options =>
            {
                var opts = new GitOptions();
                configure(opts);

                options.FetchUrl = opts.FetchUrl;
                options.UserName = opts.UserName;
                options.Password = opts.Password;
                options.EnablePolling = true;
                options.Directory = Path.Combine(_dataDirectory, "source");
                options.Tags.Use = false;
                options.Branches.Use = true;
                options.Branches.Include = new[] { opts.Branch };
            });
            return this;
        }

        /// <summary>
        ///     Sets resource URL resolver implementation
        /// </summary>
        [NotNull]
        public BlogEngineOptions UseResourceUrlResolver([NotNull] Func<IServiceProvider, IResourceUrlResolver> factory)
        {
            _builder.UseResourceUrlResolver(factory);
            return this;
        }

        /// <summary>
        ///     Sets a logger implementation
        /// </summary>
        [NotNull]
        public BlogEngineOptions UseLog([NotNull] Func<IServiceProvider, IMarkDocsLog> logFactory)
        {
            _builder.UseLog(logFactory);
            return this;
        }

        /// <summary>
        ///     Configures blog engine markdown parser and renderer
        /// </summary>
        [NotNull]
        public BlogEngineOptions ConfigureMarkdown([NotNull] Action<MarkdownOptions> configure)
        {
            configure(_markdownOptions);
            return this;
        }
    }
}
