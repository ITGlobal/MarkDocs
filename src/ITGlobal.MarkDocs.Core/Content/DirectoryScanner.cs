using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Storage;
using Microsoft.Extensions.Logging;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Content
{
    /// <summary>
    ///     Creates page tree from a content directory
    /// </summary>
    internal sealed class DirectoryScanner : IDisposable
    {
        #region fields

        private readonly ILogger _log;
        private readonly IFormat _format;
        private readonly IStorage _storage;

        private readonly IMetadataProvider[] _metadataProviders;

        #endregion

        #region .ctor

        /// <summary>
        ///     .ctor
        /// </summary>
        public DirectoryScanner(ILogger log, IFormat format, IStorage storage)
        {
            _log = log;
            _format = format;
            _storage = storage;

            _metadataProviders = new IMetadataProvider[]
            {
                new TocMetadataProvider(_log),
                new ContentMetadataProvider(format)
            };
        }

        #endregion

        #region public methods

        /// <summary>
        ///     Creates page tree from a content root directory
        /// </summary>
        /// <param name="directory">
        ///     Content root directory
        /// </param>
        /// <returns>
        ///     Page tree
        /// </returns>
        [NotNull]
        public RootDirectoryPageTreeNode ScanDirectory([NotNull] string directory)
        {
            string id;
            Metadata metadata;
            string indexPageFileName;
            List<IPageTreeNode> nodes;
            var consumedFiles = new HashSet<string>();

            ScanDirectory(
                directory,
                directory,
                consumedFiles,
                out id,
                out metadata,
                out indexPageFileName,
                out nodes);

            if (indexPageFileName != null)
            {
                consumedFiles.Add(indexPageFileName);
            }

            foreach (var node in nodes.OfType<PageTreeNode>())
            {
                node.ScanNodes((n, _) => consumedFiles.Add(n.FileName));
            }

            var attachments = CollectAttachments(directory, consumedFiles);

            var catalog = new RootDirectoryPageTreeNode(
                id: id,
                metadata: metadata,
                rootDirectory: directory,
                filename: indexPageFileName,
                relativeFileName: GetRelativePath(directory, indexPageFileName),
                nodes: nodes,
                attachments: attachments);

            return catalog;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var provider in _metadataProviders)
            {
                provider.Dispose();
            }
        }

        #endregion

        #region private methods

        private bool ScanDirectory(
            string rootDirectory,
            string path,
            HashSet<string> consumedFiles,
            out string id,
            out Metadata metadata,
            out string indexPageFileName,
            out List<IPageTreeNode> nodes)
        {
            ScanDirectorySubdirectories(rootDirectory, path, consumedFiles, out nodes);

            var pages = ScanDirectoryFiles(rootDirectory, path, out indexPageFileName);
            foreach (var filename in pages)
            {
                nodes.Add(LeafNode(rootDirectory, filename, consumedFiles));
            }

            id = GetRelativePath(rootDirectory, path);
            ResourceId.Normalize(ref id);

            metadata = null;
            if (indexPageFileName != null)
            {
                metadata = GetMetadata(indexPageFileName, consumedFiles);
            }
            metadata = metadata ?? new Metadata();
            if (string.IsNullOrEmpty(metadata.Title))
            {
                metadata.Title = Path.GetFileNameWithoutExtension(path);
            }

            if (nodes.Count == 0)
            {
                if (indexPageFileName == null)
                {
                    return false;
                }
            }

            return true;
        }

        private void ScanDirectorySubdirectories(
            string rootDirectory,
            string directory,
            HashSet<string> consumedFiles,
            out List<IPageTreeNode> nodes)
        {
            var ignoreRules = GetIgnoreRulesForDirectory(rootDirectory, directory);

            nodes = new List<IPageTreeNode>();
            foreach (var subDirectory in Directory.EnumerateDirectories(directory))
            {
                if (ignoreRules.ShouldIgnore(subDirectory))
                {
                    continue;
                }

                var node = DirectoryNode(rootDirectory, subDirectory, consumedFiles);
                if (node != null)
                {
                    nodes.Add(node);
                }
            }
        }

        private List<string> ScanDirectoryFiles(string rootDirectory, string directory, out string indexPageFileName)
        {
            var ignoreRules = GetIgnoreRulesForDirectory(rootDirectory, directory);

            var files = from filter in _format.FileFilters
                from filename in Directory.EnumerateFiles(directory, filter)
                select filename;

            var pages = new List<string>();
            var possibleIndexPages = new Dictionary<int, string>();

            foreach (var filename in files)
            {
                if (ignoreRules.ShouldIgnore(filename))
                {
                    continue;
                }

                var name = Path.GetFileName(filename);

                var matchesIndexFileName = false;
                for (var i = 0; i < _format.IndexFileNames.Length; i++)
                {
                    if (string.Equals(name, _format.IndexFileNames[i], StringComparison.OrdinalIgnoreCase))
                    {
                        possibleIndexPages[i] = filename;
                        matchesIndexFileName = true;
                        break;
                    }
                }

                if (matchesIndexFileName)
                {
                    continue;
                }

                pages.Add(filename);
            }

            indexPageFileName = null;
            if (possibleIndexPages.Count > 0)
            {
                var bestIndex = possibleIndexPages.Keys.Min();
                indexPageFileName = possibleIndexPages[bestIndex];

                possibleIndexPages.Remove(bestIndex);

                foreach (var filename in possibleIndexPages.Values)
                {
                    pages.Add(filename);
                }
            }
            return pages;
        }

        private IPageTreeNode DirectoryNode(string rootDirectory, string path, HashSet<string> consumedFiles)
        {
            string id;
            Metadata metadata;
            string indexPageFileName;
            List<IPageTreeNode> nodes;

            if (!ScanDirectory(
                rootDirectory,
                path,
                consumedFiles,
                out id,
                out metadata,
                out indexPageFileName,
                out nodes))
            {
                return null;
            }

            if (nodes.Count == 0)
            {
                if (indexPageFileName == null)
                {
                    return null;
                }

                return new LeafPageTreeNode(id, metadata, indexPageFileName,
                    GetRelativePath(rootDirectory, indexPageFileName));
            }

            return new DirectoryPageTreeNode(
                id,
                metadata,
                indexPageFileName,
                indexPageFileName != null ? GetRelativePath(rootDirectory, indexPageFileName) : null,
                nodes);
        }

        private IPageTreeNode LeafNode(string rootDirectory, string path, HashSet<string> consumedFiles)
        {
            var id = GetRelativePath(rootDirectory, path);
            id = Path.Combine(Path.GetDirectoryName(id), Path.GetFileNameWithoutExtension(id));

            var properties = GetMetadata(path, consumedFiles);
            if (string.IsNullOrEmpty(properties.Title))
            {
                properties.Title = Path.GetFileNameWithoutExtension(path);
            }

            return new LeafPageTreeNode(id, properties, path, GetRelativePath(rootDirectory, path));
        }

        internal static string GetRelativePath(string rootDirectory, string path)
        {
            if (string.Equals(rootDirectory, path, StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            rootDirectory = rootDirectory.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            if (rootDirectory[rootDirectory.Length - 1] != Path.DirectorySeparatorChar)
            {
                rootDirectory += Path.DirectorySeparatorChar;
            }

            var rootDirectoryUri = new Uri(rootDirectory);
            var pathUri = new Uri(path);

            if (rootDirectoryUri.Scheme != pathUri.Scheme)
            {
                return path;
            }

            var relativeUri = rootDirectoryUri.MakeRelativeUri(pathUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (pathUri.Scheme.Equals("file", StringComparison.OrdinalIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        private Metadata GetMetadata(string filename, HashSet<string> consumedFiles)
        {
            var properties = new Metadata();

            foreach (var provider in _metadataProviders)
            {
                var p = provider.GetMetadata(filename, consumedFiles);
                if (p != null)
                {
                    properties.CopyFrom(p);
                }
            }

            if (string.IsNullOrEmpty(properties.Title))
            {
                _log.LogWarning("'{0}' - no title found!", filename);
            }

            return properties;
        }

        private string[] CollectAttachments(string rootDirectory, HashSet<string> consumedFiles)
        {
            var files = new List<string>();
            CollectAttachments(consumedFiles, rootDirectory, rootDirectory, files);
            return files.ToArray();
        }

        private void CollectAttachments(HashSet<string> consumedFiles, string rootDirectory, string directory,
            List<string> files)
        {
            var ignoreRules = GetIgnoreRulesForDirectory(rootDirectory, directory);
            foreach (var subdirectory in Directory.EnumerateDirectories(directory))
            {
                CollectAttachments(consumedFiles, rootDirectory, subdirectory, files);
            }

            foreach (var filename in Directory.EnumerateFiles(directory))
            {
                if (consumedFiles.Contains(filename))
                {
                    continue;
                }

                if (ignoreRules.ShouldIgnore(filename))
                {
                    continue;
                }

                consumedFiles.Add(filename);
                files.Add(filename);
            }
        }

        private IIgnoreRule GetIgnoreRulesForDirectory(string rootDirectory, string path)
        {
            var ignoreRules = EnumerateIgnoreRulesForDirectory(rootDirectory, path).ToArray();
            return new IgnoreRuleList(ignoreRules);
        }

        private IEnumerable<IIgnoreRule> EnumerateIgnoreRulesForDirectory(string rootDirectory, string path)
        {
            var storageIgnorePatterns = _storage.IgnorePatterns;
            if (storageIgnorePatterns != null)
            {
                yield return new SingleIgnoreRule(rootDirectory, storageIgnorePatterns);
            }
            
            foreach (var directory in WalkDirectoriesUp(rootDirectory, path))
            {
                var mdIgnoreFileName = Path.Combine(directory, MdIgnoreFileRule.FileName);
                if (File.Exists(mdIgnoreFileName))
                {
                    yield return new MdIgnoreFileRule(directory, mdIgnoreFileName);
                }
            } 
        }

        private static IEnumerable<string> WalkDirectoriesUp(string rootDirectory, string path)
        {
            var directory = path;
            directory = Path.GetFullPath(directory);

            while (directory !=null && directory!=rootDirectory)
            {
                yield return directory;
                directory = Path.GetDirectoryName(directory);
            }
            yield return rootDirectory;
        }

        #endregion
    }
}