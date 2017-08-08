using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ITGlobal.MarkDocs.Cache
{
    /// <summary>
    ///     An implementation of <see cref="ICacheUpdateOperation"/> for <see cref="DiskCache"/>
    /// </summary>
    internal sealed class DiskCacheUpdateOperation : ICacheUpdateOperation
    {
        #region fields
        
        private readonly DiskCache _cache;
        private readonly DiskCacheDescriptor _descriptor = new DiskCacheDescriptor();

        private readonly object _contentWriteTasksLock = new object();
        private readonly List<Task> _contentWriteTasks = new List<Task>();

        private DiskCacheDescriptor _oldDescriptor;

        #endregion

        #region .ctor

        /// <summary>
        ///     .ctor
        /// </summary>

        public DiskCacheUpdateOperation(DiskCache cache, DiskCacheDescriptor descriptor)
        {
            _cache = cache;
            _descriptor = descriptor.Clone();
        }

        #endregion

        #region ICacheUpdateOperation

        /// <summary>
        ///     Clears cached content for specified documentation
        /// </summary>
        /// <param name="documentation">
        ///     Documentation
        /// </param>
        void ICacheUpdateOperation.Clear(IDocumentation documentation)
        {
            _descriptor.Items.Remove(documentation.Id);
        }

        /// <summary>
        ///     Caches content
        /// </summary>
        /// <param name="item">
        ///     Cache item
        /// </param>
        /// <param name="content">
        ///     Item content
        /// </param>
        void ICacheUpdateOperation.Write(IResource item, IResourceContent content)
        {
            var filename = _cache.GetCachedFileName(_descriptor, item);
            var directory = Path.GetDirectoryName(filename);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var contentWriteTask = Task.Run(async () =>
            {
                using (var fileStream = File.OpenWrite(filename))
                using (var contentStream = content.GetContent())
                {
                    await contentStream.CopyToAsync(fileStream);
                }
            });

            if (_cache.Options.EnableConcurrentWrites)
            {
                lock (_contentWriteTasksLock)
                {
                    _contentWriteTasks.Add(contentWriteTask);
                }
            }
            else
            {
                contentWriteTask.Wait();
            }
        }

        /// <summary>
        ///     Flushes all cached content changes
        /// </summary>
        void ICacheUpdateOperation.Flush()
        {
            lock (_contentWriteTasksLock)
            {
                if (_contentWriteTasks.Count > 0)
                {
                    Task.WaitAll(_contentWriteTasks.ToArray());
                    _contentWriteTasks.Clear();
                }
            }
            
            _descriptor.LastUpdateTime = DateTime.UtcNow;
            _descriptor.Save(_cache.DescriptorFileName, _cache.Log);
        }

        /// <summary>
        ///     Commits all cached content changes
        /// </summary>
        void ICacheUpdateOperation.Commit()
        {
            _oldDescriptor = _cache.SwapDescriptor(_descriptor);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_oldDescriptor != null)
            {
                foreach (var key in _descriptor.Items.Keys)
                {
                    _oldDescriptor.Items.Remove(key);
                }

                _cache.ReleaseDescriptor(_oldDescriptor);
            }
        }

        #endregion
    }
}