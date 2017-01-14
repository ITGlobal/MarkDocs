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
        private readonly Dictionary<string, Dictionary<string, ICacheItemContent>> _contentProviders;

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
        void ICacheUpdateOperation.Write(ICacheItem item, ICacheItemContent content)
        {
            Dictionary<string, ICacheItemContent> dict;
            if (!_contentProviders.TryGetValue(item.Documentation.Id, out dict))
            {
                dict = new Dictionary<string, ICacheItemContent>(StringComparer.OrdinalIgnoreCase);
                _contentProviders.Add(item.Documentation.Id, dict);
            }

            dict[item.Documentation.Id] = content;
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