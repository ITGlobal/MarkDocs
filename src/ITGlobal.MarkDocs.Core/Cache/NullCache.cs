using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ITGlobal.MarkDocs.Cache
{
    /// <summary>
    ///     An non-caching implementation of <see cref="ICache"/>
    /// </summary>
    internal sealed class NullCache : ICache
    {
        #region fields

        private readonly object _contentProvidersLock = new object();
        private Dictionary<string, Dictionary<string, ICacheItemContent>> _contentProviders =
            new Dictionary<string, Dictionary<string, ICacheItemContent>>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region ICache

        /// <summary>
        ///     Starts update operation
        /// </summary>
        /// <returns>
        ///     Cache update operation
        /// </returns>
        ICacheUpdateOperation ICache.BeginUpdate() => new NullCacheUpdateOperation(this);

        /// <summary>
        ///     Gets a cached content
        /// </summary>
        /// <param name="item">
        ///     Cache item
        /// </param>
        /// <returns>
        ///     Cached content or null
        /// </returns>
        public Stream Read(ICacheItem item)
        {
            var id = Path.Combine(item.Type.ToString(), item.Id);
           
            lock (_contentProvidersLock)
            {
                Dictionary<string, ICacheItemContent> dict;
                if (!_contentProviders.TryGetValue(item.Documentation.Id, out dict))
                {
                    return null;
                }

                ICacheItemContent provider;
                if (!dict.TryGetValue(id, out provider))
                {
                    return null;
                }

                return provider.GetContent();
            }
        }

        #endregion

        #region internal methods

        internal Dictionary<string, Dictionary<string, ICacheItemContent>> CloneContentProviders()
        {
            lock (_contentProvidersLock)
            {
                var clone = new Dictionary<string, Dictionary<string, ICacheItemContent>>(StringComparer.OrdinalIgnoreCase);
                foreach (var pair in _contentProviders)
                {
                    var dict = pair.Value.ToDictionary(_ => _.Key, _ => _.Value, StringComparer.OrdinalIgnoreCase);
                    clone[pair.Key] = dict;
                }

                return clone;
            }
        }

        internal void SetContentProviders(Dictionary<string, Dictionary<string, ICacheItemContent>> knownPages)
        {
            lock (_contentProvidersLock)
            {
                _contentProviders = knownPages;
            }
        }

        #endregion
    }
}