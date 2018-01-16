using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace ITGlobal.MarkDocs.Git
{
    internal sealed class GitHelper
    {
        private const string EXECUTABLE_NAME = "git";

        private readonly ILogger _log;

        public GitHelper(ILogger log)
        {
            _log = log;
        }

        public bool IsGitInstalled
        {
            get
            {
                try
                {
                    using (var exec = ExecuteNonVerbose(null, "version"))
                    {
                        exec.ThrowIfFailed();

                        var version = exec.StandardOutput.FirstOrDefault();
                        _log.LogDebug("Git is installed: {0}", version);

                        return true;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool IsRepository(string directory)
        {
            if (!Directory.Exists(directory))
            {
                return false;
            }

            if (!Directory.Exists(Path.Combine(directory, ".git")))
            {
                return false;
            }

            using (var exec = ExecuteNonVerbose(directory, "status"))
            {
                return exec.ExitCode != 128;
            }
        }

        public string GetRemoteUrl(string directory)
        {
            // $ git remote -v
            // origin  https://github.com/username/repo.git (fetch)
            // origin  https://github.com/username/repo.git (push)

            using (var exec = Execute(directory, "remote", "-v"))
            {
                exec.ThrowIfFailed();

                var originFetchUrl =
                (
                    from line in exec.StandardOutput
                    let m = Regex.Match(line, @"^\s*origin\s+([^\s]+)\s+\(fetch\)\s*$")
                    where m.Success
                    select m.Groups[1].Value
                ).FirstOrDefault();

                if (originFetchUrl == null)
                {
                    throw new InvalidOperationException("Unable to find valid remote URL");
                }

                return originFetchUrl;
            }
        }

        public string Describe(string directory)
        {
            // $ git describe --abbrev=0 --tags
            // LAST_REMOTE_TAG

            using (var exec = Execute(directory, "describe", "--abbrev=0", "--tags"))
            {
                if (exec.ExitCode != 0)
                {
                    return null;
                }

                var info = exec.StandardOutput.FirstOrDefault();
                return info;
            }
        }

        public string RevParseHead(string directory)
        {
            // $ git rev-parse --abbrev-ref HEAD
            // HEAD

            using (var exec = Execute(directory, "rev-parse", "--abbrev-ref", "HEAD"))
            {
                if (exec.ExitCode != 0)
                {
                    return null;
                }

                var info = exec.StandardOutput.FirstOrDefault();
                return info;
            }
        }

        public string LastCommitHash(string directory)
        {
            // git log -n 1 --no-merges --no-notes --pretty=format:"%H"
            // 2b8b638df38e778e661d136be5fd7dae4302d7bf

            var args = new[]
            {
                "log",
                "-n", "1",
                "--no-merges",
                "--no-notes",
                "--pretty=format:\"%H\""
            };

            using (var exec = Execute(directory, args))
            {
                exec.ThrowIfFailed();

                try
                {
                    var lines = exec.StandardError.ToArray();
                    return lines[0];
                }
                catch (Exception e)
                {
                    _log.LogWarning(0, e, "Unable to parse output of \"git log\"");
                    return null;
                }
            }
        }

        public bool LogLastCommit(
            string directory,
            out string commitSha,
            out DateTime commitTime,
            out string commitAuthor,
            out string commitMessage)
        {
            // $ git log -n 1 --no-merges --no-notes --date=iso-strict --pretty=format:"%H%n%cI%n%cn%n%s"
            // 2b8b638df38e778e661d136be5fd7dae4302d7bf
            // 2016-11-11T20:14:03-08:00
            // Connie Yau
            // Lucene.Net.Analysis.Common: Update icu version to use package that can build both x64 and x86

            var args = new[]
            {
                "log",
                "-n", "1",
                "--no-merges",
                "--no-notes",
                "--date=iso-strict",
                "--pretty=format:\"%H%n%cI%n%cn%n%s\""
            };

            using (var exec = Execute(directory, args))
            {
                exec.ThrowIfFailed();

                try
                {
                    var lines = exec.StandardOutput.ToArray();
                    commitSha = lines[0];
                    commitTime = DateTime.Parse(lines[1]);
                    commitAuthor = lines[2];
                    commitMessage = lines[3];

                    return true;
                }
                catch (Exception e)
                {
                    _log.LogWarning(0, e, "Unable to parse output of \"git log\"");

                    commitSha = null;
                    commitTime = DateTime.Today;
                    commitAuthor = null;
                    commitMessage = null;
                    return false;
                }
            }
        }

        public bool HasLocalChanges(string directory)
        {
            // $ git status --untracked --porcelain

            using (var exec = Execute(directory, "status", "--untracked", "--porcelain"))
            {
                exec.ThrowIfFailed();

                var hasAnyChanges = exec.StandardError.Any();
                return hasAnyChanges;
            }
        }

        public IEnumerable<RemoteInfo> ListRemoteBranches(string remote)
            => ListRemoteRefs("--heads", remote, @"^\s*([a-zA-Z0-9]+)\s+refs\/heads\/([^$]+)$");

        public IEnumerable<RemoteInfo> ListRemoteTags(string remote)
            => ListRemoteRefs("--tags", remote, @"^\s*([a-zA-Z0-9]+)\s+refs\/tags\/([^$]+)$");

        private IEnumerable<RemoteInfo> ListRemoteRefs(string option, string remote, [RegexPattern] string regexp)
        {
            // $ git ls-remote --quiet {option} {remote}

            using (var exec = ExecuteNonVerbose(null, "ls-remote", "--quiet", option, remote))
            {
                exec.ThrowIfFailed();

                foreach (var line in exec.StandardOutput)
                {
                    var match = Regex.Match(line, regexp);
                    if (!match.Success)
                    {
                        continue;
                    }

                    var hash = match.Groups[1].Value;
                    var name = match.Groups[2].Value;

                    yield return new RemoteInfo(name, hash);
                }
            }
        }

        public void Clone(string remote, string directory, string branch)
        {
            // $ git clone --progress -b {branch} {remote} {directory}

            using (var exec = Execute(null, "clone", "--progress", "-b", branch, remote, directory))
            {
                exec.ThrowIfFailed();
            }
        }

        public void Reset(string directory)
        {
            // $ git reset --hard HEAD

            using (var exec = Execute(directory, "reset", "--hard", "HEAD"))
            {
                exec.ThrowIfFailed();
            }
        }

        public void Clean(string directory)
        {
            // $ git clean -d --force --quiet

            using (var exec = Execute(directory, "clean", "-d", "--force", "--quiet"))
            {
                exec.ThrowIfFailed();
            }
        }

        public void Checkout(string directory, string branch)
        {
            // $ git checkout --quiet --progress {branch}

            using (var exec = Execute(directory, "checkout", "--quiet", "--progress", branch))
            {
                exec.ThrowIfFailed();
            }
        }

        public void Pull(string directory)
        {
            // $ git pull --quiet

            using (var exec = Execute(directory, "pull", "--quiet"))
            {
                exec.ThrowIfFailed();
            }
        }

        public bool FindOriginalFileInfo(
            string directory,
            string filename,
            out string commit,
            out string name
            )
        {
            // $ git log --pretty="%H" --name-only --follow {filename}

            var args = new[]
            {
                "log",
                "--pretty=\"%H\"",
                "--name-only",
                "--follow",
                filename
            };

            using (var exec = Execute(directory, args))
            {
                exec.ThrowIfFailed();

                try
                {
                    var lines = exec.StandardOutput.ToArray();
                    if (lines.Length < 2)
                    {
                        commit = null;
                        name = null;
                        return false;
                    }

                    commit = lines[lines.Length - 2];
                    name = lines[lines.Length - 1];

                    return true;
                }
                catch (Exception e)
                {
                    _log.LogWarning(0, e, "Unable to parse output of \"git log\"");

                    commit = null;
                    name = null;
                    return false;
                }
            }
        }

        public string GetBlobHash(string directory, string commit, string name)
        {
            // $ git ls-tree -r {commit} {name}

            var args = new[]
            {
                "ls-tree",
                "-r",
                commit,
                name
            };

            using (var exec = Execute(directory, args))
            {
                exec.ThrowIfFailed();

                try
                {
                    var lines = exec.StandardOutput.ToArray();

                    var match = Regex.Match(lines[0], @"^[0-9]+\s+[a-zA-Z0-9]+\s+([a-zA-Z0-9]+)\s+.*$");
                    if (!match.Success)
                    {
                        throw new Exception($"Failed to parse line \"{lines[0]}\"");
                    }

                    var hash = match.Groups[1].Value;
                    return hash;
                }
                catch (Exception e)
                {
                    _log.LogWarning(0, e, "Unable to parse output of \"git ls-tree\"");
                    return null;
                }
            }
        }

        public string GetLastChangeAutor(string directory, string path)
        {
            // $ git log -n 1 --format=%an {path}

            var args = new[]
            {
                "log",
                "-n",
                "1",
                "--format=\"%an\"",
                "--follow",
                path
            };

            using (var exec = Execute(directory, args))
            {
                exec.ThrowIfFailed();

                try
                {
                    var authorName= exec.StandardOutput.FirstOrDefault(s => !string.IsNullOrEmpty(s));
                    
                    return authorName;
                }
                catch (Exception e)
                {
                    _log.LogWarning(0, e, "Unable to parse output of \"git log\"");
                    return null;
                }
            }
        }

        private ExecuteHelper Execute(string workDirectory, params string[] args)
        {
            var execute = new ExecuteHelper(_log, EXECUTABLE_NAME, workDirectory);
            execute.Run(args);
            return execute;
        }

        private ExecuteHelper ExecuteNonVerbose(string workDirectory, params string[] args)
        {
            var execute = new ExecuteHelper(_log, EXECUTABLE_NAME, workDirectory, verboseOutput: false);
            execute.Run(args);
            return execute;
        }
    }
}