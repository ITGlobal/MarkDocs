using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using ITGlobal.MarkDocs.Storage;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace ITGlobal.MarkDocs.Cache
{
    /// <summary>
    ///     A disk-based cache implementation.
    ///     Caches pages into onto disk with minimal downtime during updates.
    /// </summary>
    internal sealed class DiskCache : ICache
    {
        #region fields

        private const string DESCRIPTOR_FILE_NAME = "cache.json";

        private readonly object _descriptorLock = new object();
        private DiskCacheDescriptor _descriptor;

        #endregion

        #region .ctor

        /// <summary>
        ///     Constructor
        /// </summary>
        public DiskCache(
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] DiskCacheOptions options)
        {
            Options = options;
            Log = loggerFactory.CreateLogger<DiskCache>();

            if (!Directory.Exists(RootDirectory))
            {
                Directory.CreateDirectory(RootDirectory);
            }

            DescriptorFileName = Path.Combine(RootDirectory, DESCRIPTOR_FILE_NAME);
            _descriptor = DiskCacheDescriptor.LoadOrCreate(DescriptorFileName, Log);

            var knownCacheDirectories = new HashSet<string>(
                _descriptor.Items.Values.Select(_ => _.Directory),
                StringComparer.OrdinalIgnoreCase);
            foreach (var path in Directory.EnumerateDirectories(RootDirectory)
                .Where(path => !knownCacheDirectories.Contains(Path.GetFileName(path))))
            {
                Log.LogDebug("Deleting abandoned cache directory '{0}'", path);

                try
                {
                    Directory.Delete(path, recursive: true);
                }
                catch (Exception e)
                {
                    Log.LogError(0, e, "Failed to delete directory '{0}'", path);
                }
            }
        }

        #endregion

        #region properties

        /// <summary>
        ///     Logger
        /// </summary>
        public ILogger Log { get; }

        /// <summary>
        ///     Root cache directory
        /// </summary>
        public string RootDirectory => Options.Directory;

        /// <summary>
        ///     Full path to cache descriptor file
        /// </summary>
        public string DescriptorFileName { get; }

        /// <summary>
        ///     Cache descriptor
        /// </summary>
        public DiskCacheDescriptor Descriptor
        {
            get
            {
                lock (_descriptorLock)
                {
                    return _descriptor;
                }
            }
        }

        /// <summary>
        ///     Configuration options
        /// </summary>
        public DiskCacheOptions Options { get; }

        #endregion

        #region ICache

        /// <summary>
        ///     Checks whether cache is up to date
        /// </summary>
        CacheVerifyResult ICache.Verify(IReadOnlyList<IContentDirectory> contentDirectories)
        {
            lock (_descriptorLock)
            {
                foreach (var contentDirectory in contentDirectories)
                {
                    if (!_descriptor.Items.TryGetValue(contentDirectory.Id, out var item) ||
                        item.Version != contentDirectory.ContentVersion.LastChangeId)
                    {
                        return CacheVerifyResult.OutOfDate;
                    }
                }

                return CacheVerifyResult.UpToDate;
            }
        }

        /// <summary>
        ///     Starts update operation
        /// </summary>
        /// <returns>
        ///     Cache update operation
        /// </returns>
        ICacheUpdateOperation ICache.BeginUpdate()
        {
            Log.LogDebug("Starting cache update operation");
            lock (_descriptorLock)
            {
                return new DiskCacheUpdateOperation(this, _descriptor.Clone());
            }
        }

        /// <summary>
        ///     Gets a cached content
        /// </summary>
        /// <param name="item">
        ///     Cache item
        /// </param>
        /// <returns>
        ///     Cached content or null
        /// </returns>
        Stream ICache.Read(IResource item)
        {
            var rootDirectory = TryGetDocumentationCacheRootDirectory(item.Documentation.Id);
            if (rootDirectory == null)
            {
                Log.LogWarning("Cache directory for '{0}' is not set", item.Documentation.Id);
                return null;
            }

            var path = GetCachedFileName(item);
            if (!File.Exists(path))
            {
                Log.LogWarning("Cache file '{0}' doesn't exist", path);
                return null;
            }

            return File.OpenRead(path);
        }

        #endregion

        #region methods

        /// <summary>
        ///     Sets active descriptor
        /// </summary>
        public DiskCacheDescriptor SwapDescriptor(DiskCacheDescriptor newDescriptor)
        {
            DiskCacheDescriptor oldDescriptor;
            lock (_descriptorLock)
            {
                oldDescriptor = _descriptor;
                _descriptor = newDescriptor;
            }

            return oldDescriptor;
        }

        /// <summary>
        ///     Deletes all files related to an old descriptor
        /// </summary>
        public void ReleaseDescriptor(DiskCacheDescriptor oldDescriptor)
        {
            string[] oldDirectories;

            lock (_descriptorLock)
            {
                oldDirectories = Directory
                    .EnumerateDirectories(RootDirectory, "*", SearchOption.TopDirectoryOnly)
                    .Where(_ => !_descriptor.Items.Any(c => string.Equals(c.Value.Directory, Path.GetFileName(_))))
                    .ToArray();
            }

            foreach (var path in oldDirectories)
            {
                Log.LogDebug("Deleting old cache directory '{0}'", path);

                try
                {
                    Directory.Delete(path, recursive: true);
                }
                catch (Exception e)
                {
                    Log.LogError(0, e, "Failed to delete directory '{0}'", path);
                }
            }
        }

        internal string GetCachedFileName(DiskCacheDescriptor currentDescriptor, IResource item)
        {
            if (!currentDescriptor.Items.TryGetValue(item.Documentation.Id, out var descriptor))
            {
                descriptor = new DiskCacheDocumentationDescriptor
                {
                    Directory = Guid.NewGuid().ToString("N")
                };
                currentDescriptor.Items.Add(item.Documentation.Id, descriptor);
            }

            if (string.IsNullOrWhiteSpace(descriptor.Directory))
            {
                descriptor.Directory = Guid.NewGuid().ToString("N");
            }

            descriptor.Version = item.Documentation.ContentVersion.LastChangeId;

            var name = item.Id == "/" ? "index" : item.Id.Substring(1);
            if (item.Type == ResourceType.Page)
            {
                name = name + ".html";
            }

            var path = Path.Combine(RootDirectory, descriptor.Directory, GetPathPrefix(item), name);
            return path;
        }

        private string GetCachedFileName(IResource item)
        {
            lock (_descriptorLock)
            {
                return GetCachedFileName(_descriptor, item);
            }
        }

        private static string GetPathPrefix(IResource item)
        {
            switch (item.Type)
            {
                case ResourceType.Page:
                    return "pages";
                case ResourceType.Attachment:
                    return "files";
                case ResourceType.Illustration:
                    return "illustrations";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string TryGetDocumentationCacheRootDirectory(string documentationId)
        {
            DiskCacheDocumentationDescriptor descriptor;

            lock (_descriptorLock)
            {
                if (!Descriptor.Items.TryGetValue(documentationId, out descriptor))
                {
                    return null;
                }
            }

            if (string.IsNullOrEmpty(descriptor.Directory))
            {
                return null;
            }

            return Path.Combine(RootDirectory, descriptor.Directory);
        }

        #endregion
    }
}