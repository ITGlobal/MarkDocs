using ITGlobal.MarkDocs.Cache.Model;
using ITGlobal.MarkDocs.Source;
using JetBrains.Annotations;
using System;
using System.IO;
using System.Security.Cryptography;

namespace ITGlobal.MarkDocs.Cache.Impl
{
    internal sealed class DiskCacheUpdateTransaction : ICacheUpdateTransaction, IAssetStoreContext
    {
        #region fields

        private readonly DiskCacheProvider _provider;
        private readonly IMarkDocsLog _log;

        private readonly ISourceTree _sourceTree;
        private readonly string _sourceHash;

        [CanBeNull]
        private readonly string _oldDirectory;
        private readonly string _newDirectory;
        private readonly string _newDirectoryName;

        [CanBeNull]
        private readonly DiskCacheIndex _oldIndex;
        private readonly DiskCacheIndex _newIndex = new DiskCacheIndex();

        private readonly bool _clearCache;

        private readonly PageAssetContainer _pageAssetContainer;
        private readonly PagePreviewAssetContainer _pagePreviewAssetContainer;
        private readonly AttachmentAssetContainer _attachmentAssetContainer;
        private readonly GeneratedAttachmentAssetContainer _generatedAssetContainer;

        private bool _isCommitted;

        #endregion

        #region .ctor

        public DiskCacheUpdateTransaction(
            DiskCacheProvider provider,
            IMarkDocsLog log,
            ISourceTree sourceTree,
            string sourceHash,
            [CanBeNull] string oldDirectory,
            [CanBeNull] DiskCacheIndex oldIndex,
            bool clearCache
            )
        {
            _provider = provider;
            _log = log;
            _sourceTree = sourceTree;
            _sourceHash = sourceHash;
            _oldDirectory = oldDirectory;
            _oldIndex = oldIndex;
            _clearCache = clearCache;

            while (true)
            {
                var bytes = new byte[16];
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(bytes);

                    _newDirectoryName = BitConverter.ToString(bytes).Replace("-", "").ToLower();

                    _newDirectory = Path.Combine(provider.RootDirectory, _newDirectoryName);
                    if (!Directory.Exists(_newDirectory))
                    {
                        break;
                    }
                }
            }

            _pageAssetContainer = new PageAssetContainer(this);
            _pagePreviewAssetContainer = new PagePreviewAssetContainer(this);
            _attachmentAssetContainer = new AttachmentAssetContainer(this);
            _generatedAssetContainer = new GeneratedAttachmentAssetContainer(this);
        }

        #endregion

        #region IAssetStoreContext

        IMarkDocsLog IAssetStoreContext.Log => _log;
        ISourceTree IAssetStoreContext.SourceTree => _sourceTree;
        string IAssetStoreContext.RootDirectory => _provider.RootDirectory;
        bool IAssetStoreContext.DisableCache => _clearCache;

        string IAssetStoreContext.OldDirectory => _oldDirectory;
        string IAssetStoreContext.NewDirectory => _newDirectory;

        DiskCacheIndex IAssetStoreContext.OldIndex => _oldIndex;
        DiskCacheIndex IAssetStoreContext.NewIndex => _newIndex;

        #endregion

        #region ICacheUpdateTransaction

        public void Store(DocumentationModel model)
        {
            var path = Path.Combine(_newDirectory, DocumentationModel.FILE_NAME);
            model.Save(path);
        }

        public void Store(PageAsset asset, Action<Stream> write) => _pageAssetContainer.Store(asset, write);

        public void Store(PagePreviewAsset asset, Action<Stream> write) => _pagePreviewAssetContainer.Store(asset, write);

        public void Store(AttachmentAsset asset, Action<Stream> write) => _attachmentAssetContainer.Store(asset, write);

        public void Store(GeneratedAsset asset, Action<Stream> write) => _generatedAssetContainer.Store(asset, write);

        public Stream Read(PageAsset asset) => _pageAssetContainer.ReadAsset(asset);

        public Stream Read(PagePreviewAsset asset) => _pagePreviewAssetContainer.ReadAsset(asset);

        public Stream Read(AttachmentAsset asset) => _attachmentAssetContainer.ReadAsset(asset);

        public Stream Read(GeneratedAsset asset) => _generatedAssetContainer.ReadAsset(asset);

        public ICacheReader Commit()
        {
            if (_oldIndex != null)
            {
                _pageAssetContainer.CopyInheritedAssets();
                _pagePreviewAssetContainer.CopyInheritedAssets();
                _attachmentAssetContainer.CopyInheritedAssets();
                _generatedAssetContainer.CopyInheritedAssets();
            }

            var path = Path.Combine(_newDirectory, DiskCacheIndex.FILE_NAME);
            _newIndex.Save(path);

            _isCommitted = true;

            var reader = new DiskCacheReader(_newDirectory, _newIndex);
            return reader;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (_isCommitted)
            {
                _provider.UpdateIndex(
                    sourceTree: _sourceTree,
                    index: _newIndex,
                    directory: _newDirectoryName,
                    sourceHash: _sourceHash
                );

                if (_oldDirectory != null)
                {
                    _provider.EnqueueForGarbageCollection(_oldDirectory);
                }
            }
        }

        #endregion
    }
}