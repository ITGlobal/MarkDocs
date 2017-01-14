using System;
using System.Collections.Generic;
using System.Linq;
using ITGlobal.MarkDocs.Storage;

namespace ITGlobal.MarkDocs.Git
{
    internal sealed class GitStorageState
    {
        public GitStorageState(IReadOnlyList<IContentDirectory> directories)
        {
            ContentDirectories = directories.ToArray();
            ContentDirectoriesById = directories.ToDictionary(
                _ => _.Id,
                _ => _,
                StringComparer.OrdinalIgnoreCase
            );
        }

        public IContentDirectory[] ContentDirectories { get; }
        public IReadOnlyDictionary<string, IContentDirectory> ContentDirectoriesById { get; }
    }
}