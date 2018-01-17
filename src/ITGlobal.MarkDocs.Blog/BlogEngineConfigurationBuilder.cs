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

        private sealed class ResourceUrlResolver : IResourceUrlResolver
        {
            private readonly string _rootUrl;

            public ResourceUrlResolver(string rootUrl)
            {
                while (rootUrl.EndsWith("/"))
                {
                    rootUrl = rootUrl.Substring(0, rootUrl.Length - 1);
                }
                _rootUrl = rootUrl;
            }

            public string ResolveUrl(IResource resource, IResource relativeTo = null)
            {
                var url = _rootUrl + resource.Id;
                return url;
            }
        }

        private readonly MarkDocsConfigurationBuilder _builder;
        private readonly string _dataDirectory;
        private readonly MarkdownOptions _markdownOptions = new MarkdownOptions();
        private string _rootUrl = "";

        internal BlogEngineConfigurationBuilder(string dataDirectory)
        {
            _builder = new MarkDocsConfigurationBuilder();
            _dataDirectory = dataDirectory;
        }

        /// <summary>
        ///     Sets blog root URL
        /// </summary>
        [PublicAPI, NotNull]
        public BlogEngineConfigurationBuilder UseRootUrl([NotNull] string url)
        {
            _rootUrl = url;
            return this;
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
        public BlogEngineConfigurationBuilder UseGitSource([NotNull] string repositoryUrl)
        {
            return UseGitSource(options =>
            {
                options.FetchUrl = repositoryUrl;
            });
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

        /// <summary>
        ///     Configures blog engine markdown parser and renderer
        /// </summary>
        [PublicAPI, NotNull]
        public BlogEngineConfigurationBuilder ConfigureMarkdown([NotNull] Action<MarkdownOptions> configure)
        {
            configure(_markdownOptions);
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
            SetupRequiredServices();

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
            
            _markdownOptions.ResourceUrlResolver = new ResourceUrlResolver(_rootUrl);
            _markdownOptions.SyntaxColorizer = new ServerHighlightJsSyntaxColorizer(Path.Combine(_dataDirectory, "temp"));
            _markdownOptions.ChildrenListRenderer = new NoneChildrenListRenderer();
            _markdownOptions.DontRenderFirstHeading = true;
            _builder.Format.UseMarkdown(_markdownOptions);

            _builder.Extensions.AddTags();
            _builder.Extensions.AddSearch(Path.Combine(_dataDirectory, "search-index"));
        }
    }
}
