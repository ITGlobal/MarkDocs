﻿using System;

namespace ITGlobal.MarkDocs.Source.Impl
{
    /// <summary>
    ///     Git content version
    /// </summary>
    internal sealed class GitSourceInfo : ISourceInfo
    {
        private GitSourceInfo(
            string sourceUrl,
            string version,
            DateTime lastChangeTime,
            string lastChangeId,
            string lastChangeDescription,
            string lastChangeAuthor)
        {
            SourceUrl = sourceUrl;
            Version = version;
            LastChangeTime = lastChangeTime;
            LastChangeId = lastChangeId;
            LastChangeDescription = lastChangeDescription;
            LastChangeAuthor = lastChangeAuthor;
        }

        /// <summary>
        ///     Source URL
        /// </summary>
        public string SourceUrl { get; }

        /// <summary>
        ///     Version name (e.g. tag name, branch name or any other identifier)
        /// </summary>
        public string Version { get; }

        /// <summary>
        ///     Last content change time
        /// </summary>
        public DateTime? LastChangeTime { get; }

        /// <summary>
        ///     Last content change identifier (e.g. commit hash)
        /// </summary>
        public string LastChangeId { get; }

        /// <summary>
        ///     Last content change description (e.g. commit message)
        /// </summary>
        public string LastChangeDescription { get; }

        /// <summary>
        ///     Last content change author (e.g. committer)
        /// </summary>
        public string LastChangeAuthor { get; }

        public static GitSourceInfo FromWorkingCopy(GitHelper git, string path)
        {
            if (!git.IsRepository(path))
            {
                throw new InvalidOperationException($"\"{path}\" is not a git repository");
            }

            var sourceUrl = git.GetRemoteUrl(path);
            if (Uri.IsWellFormedUriString(sourceUrl, UriKind.Absolute))
            {
                var sourceUrlBuilder = new UriBuilder(new Uri(sourceUrl));
                if (sourceUrlBuilder.Scheme != "git")
                {
                    sourceUrlBuilder.UserName = "";
                    sourceUrlBuilder.Password = "";
                }
                sourceUrl = sourceUrlBuilder.Uri.ToString();
            }

            var version = git.Describe(path) ?? git.RevParseHead(path);

            if (!git.LogLastCommit(path, out var commitSha, out var commitTime, out var commitAuthor, out var commitMessage))
            {
                commitSha = null;
                commitTime = DateTime.Today;
                commitAuthor = null;
                commitMessage = null;
            }

            return new GitSourceInfo(
                sourceUrl,
                version,
                commitTime,
                commitSha,
                commitMessage,
                commitAuthor
            );
        }
    }
}