using System;
using System.IO;
using ITGlobal.MarkDocs.Blog.Implementation;
using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Extensions;
using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Git;
using ITGlobal.MarkDocs.Search;
using ITGlobal.MarkDocs.Storage;
using ITGlobal.MarkDocs.Tags;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Configuration builder for Blog Engine
    /// </summary>
    public sealed class BlogEngineConfigurationBuilder
    {
        private sealed class EventConnector : IExtensionFactory, IExtension
        {
            public MarkdocsBlogEngine Engine { get; set; }

            IExtension IExtensionFactory.Create(IMarkDocService service, IMarkDocServiceState state)
            {
                Update(state);
                return this;

            }

            public void Update(IMarkDocServiceState state)
            {
                Engine?.Update(state);
            }
        }

        private readonly MarkDocsConfigurationBuilder _builder;
        private readonly string _dataDirectory;
        private readonly IResourceUrlResolver _urlResolver;

        internal BlogEngineConfigurationBuilder(string dataDirectory, IResourceUrlResolver urlResolver)
        {
            _builder = new MarkDocsConfigurationBuilder();
            _dataDirectory = dataDirectory;
            _urlResolver = urlResolver;
            SetupRequiredServices();
        }

        /// <summary>
        ///     Sets specified directory as blog data source
        /// </summary>
        [PublicAPI, NotNull]
        public BlogEngineConfigurationBuilder UseSourceDirectory([NotNull] string path, bool enableWatch = true)
        {
            _builder.Storage.UseStaticDirectory(new StaticStorageOptions
            {
                Directory = path,
                EnableWatch = enableWatch,
                UseSubdirectories = false
            });
            return this;
        }

        /// <summary>
        ///     Configures blog engine to use git as blog data source
        /// </summary>
        [PublicAPI, NotNull]
        public BlogEngineConfigurationBuilder UseGitSource([NotNull] Action<GitOptions> configure)
        {
            _builder.Storage.UseGit(options =>
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

        internal Func<IServiceProvider, IBlogEngine> Build()
        {
            return services =>
            {
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                return BuildInstance(loggerFactory);
            };
        }

        internal IBlogEngine BuildInstance(ILoggerFactory loggerFactory)
        {
            var connector = new EventConnector();
            _builder.Extensions.Add(_ => connector);
            var markdocs = _builder.BuildInstance(loggerFactory);
            var engine = new MarkdocsBlogEngine(markdocs, loggerFactory);
            connector.Engine = engine;
            return engine;
        }

        private void SetupRequiredServices()
        {
            _builder.Cache.UseDisk(new DiskCacheOptions
            {
                Directory = Path.Combine(_dataDirectory, "cache"),
                EnableConcurrentWrites = false
            });

            _builder.Format.UseMarkdown(new MarkdownOptions
            {
                ResourceUrlResolver = _urlResolver,
                SyntaxColorizer = new ServerHighlightJsSyntaxColorizer(Path.Combine(_dataDirectory, "temp")),
                ChildrenListRenderer = new NoneChildrenListRenderer(),
                DontRenderFirstHeading = true
            });

            _builder.Extensions.AddTags();
            _builder.Extensions.AddSearch(Path.Combine(_dataDirectory, "search-index"));
        }
    }
}
