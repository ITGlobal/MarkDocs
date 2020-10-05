using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITGlobal.MarkDocs.Extensions;

namespace ITGlobal.MarkDocs.Tags.Impl
{
    /// <summary>
    ///     Tags extension
    /// </summary>
    internal sealed class TagsExtension : ITagService, IExtension
    {

        #region fields

        private static readonly string[] EmptyTags = new string[0];
        private static readonly IPage[] EmptyPageList = new IPage[0];

        private readonly object _stateLock = new object();
        private TagsExtensionState _state = TagsExtensionState.Empty;
        private TagsExtensionState _tempState;

        #endregion

        #region ITagService

        /// <summary>
        ///     Gets a list of all known tags
        /// </summary>
        /// <param name="documentation">
        ///     Documentation
        /// </param>
        /// <returns>
        ///     List of tags
        /// </returns>
        public IReadOnlyList<string> GetTags(IDocumentation documentation)
        {
            lock (_stateLock)
            {
                var branch = _state.GetBranch(documentation);
                return (IReadOnlyList<string>) branch?.Tags ?? Array.Empty<string>();
            }
        }

        /// <summary>
        ///     Gets a list of normalized page tags
        /// </summary>
        /// <param name="page">
        ///     Page
        /// </param>
        /// <returns>
        ///     List of tags
        /// </returns>
        public IReadOnlyList<string> GetPageTags(IPage page)
        {
            lock (_stateLock)
            {
                var branch = _state.GetBranch(page.Documentation);
                if (branch != null && branch.Pages.TryGetValue(page.Id, out var node))
                {
                    return node.Tags;
                }

                return Array.Empty<string>();
            }
        }

        /// <summary>
        ///     Gets a list of pages with specified tag
        /// </summary>
        /// <param name="documentation">
        ///     Documentation
        /// </param>
        /// <param name="tag">
        ///     Tag
        /// </param>
        /// <returns>
        ///     List of pages
        /// </returns>
        public IReadOnlyList<IPage> GetPagesByTag(IDocumentation documentation, string tag)
        {
            return GetPagesByTags(documentation, includeTags: new[] {tag});
        }

        /// <summary>
        ///     Gets a list of pages with tags
        /// </summary>
        /// <param name="documentation">
        ///     Documentation
        /// </param>
        /// <param name="includeTags">
        ///     Include only pages with specified tags
        /// </param>
        /// <param name="excludeTags">
        ///     Exclude pages with specified tags
        /// </param>
        /// <returns>
        ///     List of pages
        /// </returns>
        public IReadOnlyList<IPage> GetPagesByTags(
            IDocumentation documentation,
            string[] includeTags = null,
            string[] excludeTags = null)
        {
            includeTags = NormalizeTags(includeTags);
            excludeTags = NormalizeTags(excludeTags);

            TagsExtensionStateBranch branch;
            lock (_stateLock)
            {
                branch = _state.GetBranch(documentation);
            }

            if (branch == null)
            {
                return Array.Empty<IPage>();
            }

            var query = branch.Pages.AsEnumerable();

            if (includeTags != null && includeTags.Length > 0)
            {
                foreach (var tag in includeTags)
                {
                    query = query.Where(_ => _.Value.HasTag(tag));
                }
            }

            if (excludeTags != null && excludeTags.Length > 0)
            {
                foreach (var tag in excludeTags)
                {
                    query = query.Where(_ => !_.Value.HasTag(tag));
                }
            }

            var pages = query.Select(_ => _.Value.Page).ToList();
            return pages;
        }

        #endregion

        #region IExtension

        public void Initialize(IMarkDocState state)
        {
            var newState = TagsExtensionState.Empty;
            foreach (var documentation in state.List)
            {
                newState = newState.AddOrUpdate(documentation);
            }

            lock (_stateLock)
            {
                _state = newState;
            }
        }

        public void OnCreated(IDocumentation documentation)
        {
            TagsExtensionState state;
            lock (_stateLock)
            {
                state = _state;
            }

            state = state.AddOrUpdate(documentation);

            lock (_stateLock)
            {
                _state = state;
            }
        }

        public void OnUpdated(IDocumentation documentation)
        {
            TagsExtensionState state;
            lock (_stateLock)
            {
                state = _tempState ?? _state;
            }

            state = state.AddOrUpdate(documentation);

            lock (_stateLock)
            {
                _tempState = state;
            }
        }

        public void OnUpdateCompleted(IDocumentation documentation)
        {
            lock (_stateLock)
            {
                _state = _tempState ?? _state;
                _tempState = null;
            }
        }

        public void OnRemoved(IDocumentation documentation)
        {
            TagsExtensionState state;
            lock (_stateLock)
            {
                state = _state;
            }

            state = state.AddOrUpdate(documentation);

            lock (_stateLock)
            {
                _state = state;
            }
        }

        #endregion

        #region internal methods

        internal static string NormalizeTag(string tag)
        {
            var builder = new StringBuilder();

            foreach (var c in tag)
            {
                if (char.IsLetterOrDigit(c))
                {
                    builder.Append(char.ToLowerInvariant(c));
                }
                else if (char.IsPunctuation(c) || char.IsWhiteSpace(c))
                {
                    if (builder.Length > 0 && !char.IsPunctuation(builder[builder.Length - 1]))
                    {
                        builder.Append('-');
                    }
                }
            }

            while (builder.Length > 0 && char.IsPunctuation(builder[builder.Length - 1]))
            {
                builder.Remove(builder.Length - 1, 1);
            }

            return builder.ToString();
        }

        internal static string[] NormalizeTags(string[] tags)
        {
            if (tags == null || tags.Length == 0)
            {
                return Array.Empty<string>();
            }

            return tags.Select(NormalizeTag).ToArray();
        }

        #endregion

    }
}