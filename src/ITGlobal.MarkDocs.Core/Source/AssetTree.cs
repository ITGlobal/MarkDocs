using System.Collections.Generic;
using System.Collections.Immutable;
using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Source.Impl;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Source
{
    /// <summary>
    ///     Asset tree
    /// </summary>
    [PublicAPI]
    public sealed class AssetTree
    {
        private readonly Dictionary<string, Asset> _assetsById
            = new Dictionary<string, Asset>();

        private readonly Dictionary<string, BranchPageAsset> _parentPages
            = new Dictionary<string, BranchPageAsset>();

        /// <summary>
        ///     .ctor
        /// </summary>
        public AssetTree(
            [NotNull] string id,
            [NotNull] string rootDirectory,
            [NotNull] ISourceInfo sourceInfo,
            [NotNull] PageAsset rootPage,
            [NotNull] FileAsset[] files)
        {
            Id = id;
            RootDirectory = rootDirectory;
            SourceInfo = sourceInfo;
            RootPage = rootPage;
            Files = files.ToImmutableDictionary(_ => _.Id);

            var pages = ImmutableDictionary.CreateBuilder<string, PageAsset>();
            Walk(rootPage);
            Pages = pages.ToImmutable();

            foreach (var a in files)
            {
                _assetsById[a.Id] = a;
            }

            void Walk(PageAsset page, BranchPageAsset parent = null)
            {
                pages[page.Id] = page;

                if (parent != null)
                {
                    _parentPages[page.Id] = parent;
                }

                _assetsById[page.Id] = page;

                if (page is BranchPageAsset branchPage)
                {
                    foreach (var subpage in branchPage.Subpages)
                    {
                        Walk(subpage, branchPage);
                    }
                }
            }
        }

        /// <summary>
        ///     Source tree ID
        /// </summary>
        [NotNull]
        public string Id { get; }

        /// <summary>
        ///     Source tree root directory
        /// </summary>
        [NotNull]
        public string RootDirectory { get; }

        /// <summary>
        ///     Content version information
        /// </summary>
        [NotNull]
        public ISourceInfo SourceInfo { get; }

        /// <summary>
        ///     Root page asset
        /// </summary>
        [NotNull]
        public PageAsset RootPage { get; }

        /// <summary>
        ///     Page assets by ID
        /// </summary>
        [NotNull]
        public ImmutableDictionary<string, PageAsset> Pages { get; }

        /// <summary>
        ///     File assets by ID
        /// </summary>
        [NotNull]
        public ImmutableDictionary<string, FileAsset> Files { get; }

        /// <summary>
        ///     Gets an assets by its ID
        /// </summary>
        [CanBeNull]
        public Asset TryGetAsset([NotNull] string id)
        {
            _assetsById.TryGetValue(id, out var asset);
            return asset;
        }

        /// <summary>
        ///     Gets a parent page asset for a specified page asset ID
        /// </summary>
        [CanBeNull]
        public BranchPageAsset TryGetParentPage([NotNull] string id)
        {
            _parentPages.TryGetValue(id, out var parentPage);
            return parentPage;
        }

        /// <summary>
        ///     Runs page content validation
        /// </summary>
        internal void Validate(PageValidateContext ctx)
        {
            foreach (var (_, pageAsset) in Pages)
            {
                ctx.Page = pageAsset;
                pageAsset.Content.Validate(ctx, this);
            }
        }
    }

    internal sealed class PageValidateContext : IPageValidateContext
    {   
        private readonly IShallowPageAssetReader _worker;

        public PageValidateContext(IShallowPageAssetReader worker)
        {
            _worker = worker;
        }
        
        public PageAsset Page { get; set; }

        IResourceId IPageValidateContext.Page => Page;

        void IPageValidateContext.Warning(string message, int? lineNumber)
        {
            _worker.Report.Warning(Page.AbsolutePath, message, lineNumber);
        }

        void IPageValidateContext.Error(string message, int? lineNumber)
        {
            _worker.Report.Error(Page.AbsolutePath, message, lineNumber);
        }

    }
}