using System.Collections.Generic;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Source
{
    public sealed class AssetTree
    {
        private readonly Dictionary<string, Asset> _assetsById
            = new Dictionary<string, Asset>();

        private readonly Dictionary<string, BranchPageAsset> _parentPages
            = new Dictionary<string, BranchPageAsset>();

        public AssetTree(
            [NotNull] string id,
            [NotNull] ISourceInfo sourceInfo,
            [NotNull] PageAsset rootPage,
            [NotNull] AttachmentAsset[] attachments)
        {
            Id = id;
            SourceInfo = sourceInfo;
            RootPage = rootPage;
            Attachments = attachments;

            Walk(rootPage);
            foreach (var a in attachments)
            {
                _assetsById[a.Id] = a;
            }

            void Walk(PageAsset page, BranchPageAsset parent = null)
            {
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

        [NotNull]
        public string Id { get; }

        [NotNull]
        public ISourceInfo SourceInfo { get; }

        [NotNull]
        public PageAsset RootPage { get; }

        [NotNull]
        public AttachmentAsset[] Attachments { get; }

        [CanBeNull]
        public Asset TryGetAsset([NotNull] string id)
        {
            _assetsById.TryGetValue(id, out var asset);
            return asset;
        }

        [CanBeNull]
        public BranchPageAsset TryGetParentPage([NotNull] string id)
        {
            _parentPages.TryGetValue(id, out var parentPage);
            return parentPage;
        }
    }
}