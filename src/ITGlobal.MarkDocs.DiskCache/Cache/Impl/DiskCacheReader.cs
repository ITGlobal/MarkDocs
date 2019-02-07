using System;
using System.IO;

namespace ITGlobal.MarkDocs.Cache.Impl
{
    internal sealed class DiskCacheReader : ICacheReader
    {
        private readonly string _directory;
        private readonly DiskCacheIndex _index;

        public DiskCacheReader(string directory, DiskCacheIndex index)
        {
            _directory = directory;
            _index = index;
        }

        public Stream Read(IResource resource)
        {
            DiskCacheIndex.Dictionary dictionary;
            switch (resource.Type)
            {
                case ResourceType.Page:
                    dictionary = _index.Pages;
                    break;
                case ResourceType.PagePreview:
                    dictionary = _index.PagePreviews;
                    break;
                case ResourceType.File:
                    dictionary = _index.Files;
                    break;
                case ResourceType.GeneratedFile:
                    dictionary = _index.GeneratedFiles;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Resource type \"{resource.Type}\" is not supported");
            }

            if (!dictionary.TryGetValue(resource.Id, out var item))
            {
                throw new CachedAssetNotFoundException($"Resource \"{resource.Id}\" is not found in cache");
            }

            var path = Path.Combine(_directory, item.Filename);
            try
            {
                var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                return stream;
            }
            catch (FileNotFoundException e)
            {
                throw new CachedAssetNotFoundException($"Resource \"{resource.Id}\" is not found in cache", e);
            }
        }
    }
}