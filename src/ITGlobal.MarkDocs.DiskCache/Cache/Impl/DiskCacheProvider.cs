using ITGlobal.MarkDocs.Cache.Model;
using ITGlobal.MarkDocs.Impl;
using ITGlobal.MarkDocs.Source;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ITGlobal.MarkDocs.Cache.Impl
{
    // ------------------------------------------------------------------------
    //
    // DiskCache
    //
    // ------------------------------------------------------------------------
    //
    // DiskCache uses a directory tree to store compilation output.
    //
    // Directory structure
    // -------------------
    //
    // root/
    //   +--/{dir-id}/
    //   |    |
    //   |    +--/data/
    //   |    |    |
    //   |    |    +----/{file-id}
    //   |    |
    //   |    +--/model.json
    //   |    |
    //   |    +--/index.json
    //   | 
    //   +--/cache.json
    //
    // Files
    // -----
    //
    // - /cache.json - stores a `DiskCacheDescriptor` model that contains information about
    //   per-branch subdirectories.
    //   Provides "Documentation Branch ID" to "Directory Name" mapping.
    // - /{id}/model.json - stores a `DocumentationModel` model that stores serialized 
    //   documentation info - pages, files, theis relationships nad version information.
    //   This file is not used directly by DiskCache.
    // - /{id}/index.json - stores a `DiskCacheIndex` model that stores information
    //   about every cached asset including its hash and cache file name.
    // - /{id}/data/{file-id} - stores a cached asset. File name is auto generated
    //   and doesn't relate directly to asset name or type.
    //   All "asset-cache file" relation information is stored in corresponding
    //   index.json file.
    //
    // ------------------------------------------------------------------------
    internal sealed class DiskCacheProvider : ICacheProvider, IDisposable
    {
        #region consts

        private const int GARBAGE_COLLECTOR_SLEEP_TIME = 60 * 1000 /* 1min */;

        #endregion

        #region fields

        private readonly DiskCacheOptions _options;
        private readonly IMarkDocsLog _log;

        private readonly string _descriptorFilePath;

        private readonly object _cacheLock = new object();
        private DiskCacheDescriptor _descriptor;

        private readonly Dictionary<string, DiskCacheIndex> _indices;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly Task _backgroundGarbageCollector;

        private readonly object _garbageLock = new object();
        private HashSet<string> _garbage = new HashSet<string>();

        #endregion

        #region .ctor

        public DiskCacheProvider(DiskCacheOptions options, IMarkDocsLog log)
        {
            _options = options;
            _log = log;

            Directory.CreateDirectory(RootDirectory);
            _descriptorFilePath = Path.Combine(RootDirectory, DiskCacheDescriptor.FILE_NAME);

            lock (_cacheLock)
            {
                (_descriptor, _indices) = Initialize(_descriptorFilePath, RootDirectory, log);

                CollectGarbage();
            }

            _backgroundGarbageCollector = Task.Run(GarbageCollectionWorkerAsync);
        }

        #endregion

        #region ICacheProvider

        public CacheDocumentationModel[] Load()
        {
            return Iterator().ToArray();

            IEnumerable<CacheDocumentationModel> Iterator()
            {
                DiskCacheDescriptor descriptor;
                lock (_cacheLock)
                {
                    descriptor = _descriptor;
                }

                foreach (var (id, item) in descriptor.Items)
                {
                    DiskCacheIndex index;
                    lock (_cacheLock)
                    {
                        if (!_indices.TryGetValue(id, out index))
                        {
                            continue;
                        }
                    }

                    var directory = Path.Combine(RootDirectory, item.Directory);
                    var path = Path.Combine(directory, DocumentationModel.FILE_NAME);
                    var model = DocumentationModel.Load(path, _log);
                    if (model != null)
                    {
                        model.Id = id;
                        var reader = new DiskCacheReader(directory, index);
                        yield return new CacheDocumentationModel(model, reader);
                    }
                }
            }
        }

        public ICacheUpdateTransaction BeginTransaction(ISourceTree sourceTree, ISourceInfo sourceInfo, bool forceCacheClear = false)
        {
            var newSourceHash = HashUtil.HashObject(new { url = sourceInfo.SourceUrl });

            DiskCacheIndex oldIndex = null;
            string oldDirectory = null;
            lock (_cacheLock)
            {
                if (_descriptor.Items.TryGetValue(sourceTree.Id, out var descriptorItem))
                {
                    if (_indices.TryGetValue(sourceTree.Id, out oldIndex))
                    {
                        oldDirectory = descriptorItem.Directory;

                        // If documentation source URL has changed - a cache cleanup will be required
                        forceCacheClear |= !string.Equals(
                            descriptorItem.Hash,
                            newSourceHash,
                            StringComparison.OrdinalIgnoreCase
                        );
                    }
                }
            }

            var transaction = new DiskCacheUpdateTransaction(
                provider: this,
                log: _log,
                sourceTree: sourceTree,
                sourceHash: newSourceHash,
                oldDirectory: oldDirectory,
                oldIndex: oldIndex,
                forceCacheClear
            );

            return transaction;
        }

        public void Drop(string documentationId)
        {
            string directory = null;
            lock (_cacheLock)
            {
                if (_descriptor.Items.TryGetValue(documentationId, out var descriptorItem))
                {
                    directory = descriptorItem.Directory;

                    _descriptor.Items.Remove(documentationId);
                    _descriptor.Save(_descriptorFilePath);
                }

                _indices.Remove(documentationId);
            }

            if (directory != null)
            {
                EnqueueForGarbageCollection(directory);
            }
        }

        public Stream Read(IDocumentation documentation, IResource resource)
        {
            DiskCacheDescriptor.Item descriptorItem;
            DiskCacheIndex index;
            lock (_cacheLock)
            {
                if (!_descriptor.Items.TryGetValue(documentation.Id, out descriptorItem))
                {
                    throw new CachedAssetNotFoundException($"Cache index for \"{documentation.Id}\" is not found");
                }

                if (!_indices.TryGetValue(documentation.Id, out index))
                {
                    throw new CachedAssetNotFoundException($"Cache index for \"{documentation.Id}\" is not found");
                }
            }

            DiskCacheIndex.Dictionary dictionary;
            switch (resource.Type)
            {
                case ResourceType.Page:
                    dictionary = index.Pages;
                    break;
                case ResourceType.PagePreview:
                    dictionary = index.PagePreviews;
                    break;
                case ResourceType.Attachment:
                    dictionary = index.Files;
                    break;
                case ResourceType.GeneratedAttachment:
                    dictionary = index.GeneratedFiles;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Resource type \"{resource.Type}\" is not supported");
            }

            if (!dictionary.TryGetValue(resource.Id, out var item))
            {
                throw new CachedAssetNotFoundException($"Resource \"{resource.Id}\" is not found in cache");
            }

            var path = Path.Combine(RootDirectory, descriptorItem.Directory, item.Filename);
            try
            {
                var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                return stream;
            }
            catch (FileNotFoundException e)
            {
                throw new CachedAssetNotFoundException($"Resource \"{resource.Id}\" is not found in cache", e);
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            _cts.Cancel();
            _backgroundGarbageCollector.Wait();
        }

        #endregion

        #region internal members

        internal string RootDirectory => _options.Directory;

        internal void UpdateIndex(ISourceTree sourceTree, DiskCacheIndex index, string directory, string sourceHash)
        {
            lock (_cacheLock)
            {
                _indices[sourceTree.Id] = index;
                _descriptor.Update(sourceTree.Id, directory, sourceHash);
                _descriptor.Save(_descriptorFilePath);
            }
        }

        internal void EnqueueForGarbageCollection(string directory)
        {
            lock (_garbageLock)
            {
                _garbage.Add(directory);
            }
        }

        #endregion

        #region private members

        private void CollectGarbage()
        {
            var knownDirectoryNames = new HashSet<string>();
            lock (_cacheLock)
            {
                foreach (var (_, item) in _descriptor.Items)
                {
                    knownDirectoryNames.Add(item.Directory);
                }
            }

            foreach (var directory in Directory.EnumerateDirectories(RootDirectory, "*", SearchOption.TopDirectoryOnly))
            {
                if (!knownDirectoryNames.Contains(Path.GetFileName(directory)))
                {
                    _log.Warning($"Found garbage directory '{directory}', it will be deleted");
                    EnqueueForGarbageCollection(directory);
                }
            }
        }

        private static (DiskCacheDescriptor, Dictionary<string, DiskCacheIndex>) Initialize(
            string descriptorFilePath,
            string rootDirectory,
            IMarkDocsLog log)
        {
            var descriptor = DiskCacheDescriptor.LoadOrCreate(descriptorFilePath, log);
            var indices = new Dictionary<string, DiskCacheIndex>(StringComparer.OrdinalIgnoreCase);

            var damagedItemIds = new List<string>();
            foreach (var (id, item) in descriptor.Items)
            {
                var path = Path.Combine(rootDirectory, item.Directory, DiskCacheIndex.FILE_NAME);
                var index = DiskCacheIndex.Load(path, log);
                if (index == null)
                {
                    log.Warning($"Cache descriptor is damaged: cache index for '{id}' is missing");
                    damagedItemIds.Add(id);
                    continue;
                }

                indices[id] = index;
            }

            if (damagedItemIds.Count > 0)
            {
                foreach (var id in damagedItemIds)
                {
                    descriptor.Items.Remove(id);
                }

                descriptor.Save(descriptorFilePath);
                log.Info("Cache descriptor has been recovered");
            }

            return (descriptor, indices);
        }

        private async Task GarbageCollectionWorkerAsync()
        {
            var cancellationToken = _cts.Token;

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    RunGarbageCollection();

                    await Task.Delay(GARBAGE_COLLECTOR_SLEEP_TIME, cancellationToken);
                }

            }
            catch (OperationCanceledException) { }
        }

        private void RunGarbageCollection()
        {
            string[] directories;
            lock (_garbageLock)
            {
                if (_garbage.Count == 0)
                {
                    return;
                }

                directories = _garbage.ToArray();
                _garbage.Clear();
            }

            foreach (var directory in directories)
            {
                try
                {
                    if (Directory.Exists(directory))
                    {
                        Directory.Delete(directory, recursive: true);
                    }
                }
                catch (IOException e)
                {
                    _log.Error($"Unable to delete garbage directory '{directory}', will try again. {e.Message}");
                    EnqueueForGarbageCollection(directory);
                }
                catch (Exception e)
                {
                    _log.Error(e, $"Unable to delete garbage directory '{directory}'");
                }
            }
        }

        #endregion
    }
}
