using System;
using System.Collections.Generic;

namespace ITGlobal.MarkDocs.Cache
{
    /// <summary>
    ///     An implementation of <see cref="ICacheUpdateOperation"/> for <see cref="NullCache"/>
    /// </summary>
    internal sealed class NullCacheUpdateOperation : ICacheUpdateOperation
    {
        #region fields

        private readonly NullCache _cache;
        private readonly Dictionary<string, Dictionary<string, IResourceContent>> _contentProviders;

        #endregion

        #region .ctor

        /// <summary>
        ///     .ctor
        /// </summary>
        public NullCacheUpdateOperation(NullCache cache)
        {
            _cache = cache;
            _contentProviders = cache.CloneContentProviders();
        }

        #endregion

        #region ICacheUpdateOperation

        /// <summary>
        ///     Clears cached content for specified documentation
        /// </summary>
        /// <param name="documentation">
        ///     Documentation
        /// </param>
        void ICacheUpdateOperation.Clear(IDocumentation documentation) => _contentProviders.Remove(documentation.Id);

        /// <summary>
        ///     Caches content
        /// </summary>
        /// <param name="item">
        ///     Cache item
        /// </param>
        /// <param name="content">
        ///     Item content
        /// </param>
        /// <param name="callback">
        ///     A callback that is called after cache item is written into cache
        /// </param>
        void ICacheUpdateOperation.Write(IResource item, IResourceContent content, Action callback)
        {
            Dictionary<string, IResourceContent> dict;
            if (!_contentProviders.TryGetValue(item.Documentation.Id, out dict))
            {
                dict = new Dictionary<string, IResourceContent>(StringComparer.OrdinalIgnoreCase);
                _contentProviders.Add(item.Documentation.Id, dict);
            }

            dict[item.Documentation.Id] = content;
            callback();
        }

        /// <summary>
        ///     Flushes all cached content changes
        /// </summary>
        void ICacheUpdateOperation.Flush() { }

        /// <summary>
        ///     Commits all cached content changes
        /// </summary>
        void ICacheUpdateOperation.Commit() => _cache.SetContentProviders(_contentProviders);

        /// <inheritdoc />
        void IDisposable.Dispose() { }

        #endregion
    }
}