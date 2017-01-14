using System;
using ITGlobal.MarkDocs;

namespace ITGlobal.MarkDocs.Git
{
    /// <summary>
    ///     Git content version
    /// </summary>
    internal sealed class GitContentVersion : IContentVersion
    {
        private GitContentVersion(
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
        public DateTime LastChangeTime { get; }

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

        public static GitContentVersion FromWorkingCopy(GitHelper git, string id, string path)
        {
            if (!git.IsRepository(path))
            {
                throw new InvalidOperationException($"\"{path}\" is not a git repository");
            }

            var sourceUrl = git.GetRemoteUrl(path);
            var version = git.Describe(path) ?? git.RevParseHead(path);

            string commitSha;
            DateTime commitTime;
            string commitAuthor;
            string commitMessage;

            if (!git.LogLastCommit(path, out commitSha, out commitTime, out commitAuthor, out commitMessage))
            {
                commitSha = null;
                commitTime = DateTime.Today;
                commitAuthor = null;
                commitMessage = null;
            }

            return new GitContentVersion(
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