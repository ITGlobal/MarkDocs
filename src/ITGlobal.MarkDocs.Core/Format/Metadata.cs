using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     Page metadata
    /// </summary>
    [PublicAPI]
    public sealed class Metadata
    {
        #region fields
        
        private static readonly string[] EmptyArray = new string[0];
        private static readonly Dictionary<string, string> EmptyDictionary = new Dictionary<string, string>();
        
        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();

        #endregion

        #region metadata

        /// <summary>
        ///     Page permanent identifier
        /// </summary>
        [PublicAPI]
        public string ContentId
        {
            get => GetString(nameof(ContentId), "");
            set => SetString(nameof(ContentId), value);
        }

        /// <summary>
        ///     Page's last change author
        /// </summary>
        [PublicAPI]
        public string LastChangedBy
        {
            get => GetString(nameof(LastChangedBy));
            set => SetString(nameof(LastChangedBy), value);
        }

        /// <summary>
        ///     Page title
        /// </summary>
        [PublicAPI]
        public string Title
        {
            get => GetString(nameof(Title), "");
            set => SetString(nameof(Title), value);
        }

        /// <summary>
        ///     Page description
        /// </summary>
        [PublicAPI]
        public string Description
        {
            get => GetString(nameof(Description), "");
            set => SetString(nameof(Description), value);
        }

        /// <summary>
        ///     Page order
        /// </summary>
        [PublicAPI]
        public int Order
        {
            get => GetInteger(nameof(Order), 0);
            set => SetInteger(nameof(Order), value);
        }

        /// <summary>
        ///     Page tags
        /// </summary>
        [PublicAPI]
        public IReadOnlyList<string> Tags
        {
            get => GetArray(nameof(Tags), EmptyArray);
            set => SetArray(nameof(Tags), value, merge: false);
        }

        /// <summary>
        ///     Page meta tags (HTML)
        /// </summary>
        [PublicAPI]
        public IReadOnlyList<string> MetaTags
        {
            get => GetArray(nameof(MetaTags), EmptyArray);
            set => SetArray(nameof(MetaTags), value, merge: false);
        }

        #endregion

        #region getters

        /// <summary>
        ///     Gets a string property
        /// </summary>
        public string GetString([NotNull] string key, string defaultValue = null)
        {
            _data.TryGetValue(key, out var raw);
            switch (raw)
            {
                case string value:
                    return value;
                default:
                    return defaultValue;
            }
        }

        /// <summary>
        ///     Gets an integer property
        /// </summary>
        public int GetInteger([NotNull] string key, int defaultValue = 0)
        {
            _data.TryGetValue(key, out var raw);
            switch (raw)
            {
                case int value:
                    return value;
                default:
                    return defaultValue;
            }
        }

        /// <summary>
        ///     Gets an arrayproperty
        /// </summary>
        public IReadOnlyList<string> GetArray([NotNull] string key, IReadOnlyList<string> defaultValue = null)
        {
            _data.TryGetValue(key, out var raw);
            switch (raw)
            {
                case IReadOnlyList<string> value:
                    return value;
                default:
                    return defaultValue;
            }
        }

        /// <summary>
        ///     Gets a dictinary property
        /// </summary>
        public IReadOnlyDictionary<string, string> GetDictionary([NotNull] string key, IReadOnlyDictionary<string, string> defaultValue = null)
        {
            _data.TryGetValue(key, out var raw);
            switch (raw)
            {
                case IReadOnlyDictionary<string, string> value:
                    return value;
                default:
                    return defaultValue;
            }
        }

        #endregion

        #region setters

        /// <summary>
        ///     Sets a string property
        /// </summary>
        public void SetString([NotNull] string key, [NotNull] string value)
        {
            _data[key] = value;
        }

        /// <summary>
        ///     Sets an integerproperty
        /// </summary>
        public void SetInteger([NotNull] string key, int value)
        {
            _data[key] = value;
        }

        /// <summary>
        ///     Sets an array string property
        /// </summary>
        public void SetArray([NotNull] string key, [NotNull] IReadOnlyList<string> values, bool merge)
        {
            if (!merge)
            {
                _data[key] = values;
                return;
            }

            var existingValues = GetArray(key);
            if (existingValues != null)
            {
                _data[key] = existingValues.Concat(values).ToArray();
            }
            else
            {
                _data[key] = values;
            }
        }

        /// <summary>
        ///     Sets a dictionary property
        /// </summary>
        public void SetDictionary([NotNull] string key, [NotNull] IReadOnlyDictionary<string, string> values, bool merge)
        {
            if (!merge)
            {
                _data[key] = values;
                return;
            }

            var existingValues = GetDictionary(key);
            if (existingValues != null)
            {
                var dict = new Dictionary<string, string>();
                foreach (var pair in existingValues)
                {
                    dict[pair.Key] = pair.Value;
                }
                foreach (var pair in values)
                {
                    dict[pair.Key] = pair.Value;
                }
                _data[key] = dict;
            }
            else
            {
                _data[key] = values;
            }
        }

        #endregion

        #region internal methods

        /// <summary>
        ///     Copies page metadata from <paramref name="source"/>
        /// </summary>
        internal void CopyFrom([NotNull] Metadata source)
        {
            foreach (var pair in source._data)
            {
                if (!_data.TryGetValue(pair.Key, out var existing))
                {
                    _data[pair.Key] = pair.Value;
                    continue;
                }

                switch (pair.Value)
                {
                    case string _:
                        if (!(existing is string) || !string.IsNullOrEmpty((string) existing))
                        {
                            _data[pair.Key] = pair.Value;
                        }
                        break;
                    case int _:
                        if (!(existing is int))
                        {
                            _data[pair.Key] = pair.Value;
                        }
                        break;

                    case IReadOnlyList<string> strs:
                        if (!(existing is IReadOnlyList<string> existingArray))
                        {
                            _data[pair.Key] = pair.Value;
                        }
                        else
                        {
                            _data[pair.Key] = existingArray.Concat(strs).ToArray();
                        }
                        break;
                    case IReadOnlyDictionary<string, string> dict:
                        if (!(existing is IReadOnlyDictionary<string, string> existingDict))
                        {
                            _data[pair.Key] = pair.Value;
                        }
                        else
                        {
                            var data = new Dictionary<string, string>();
                            foreach (var p in existingDict)
                            {
                                data[p.Key] = p.Value;
                            }
                            foreach (var p in dict)
                            {
                                data[p.Key] = p.Value;
                            }
                            _data[pair.Key] = data;
                        }
                        break;

                }
            }
        }

        #endregion
    }
}