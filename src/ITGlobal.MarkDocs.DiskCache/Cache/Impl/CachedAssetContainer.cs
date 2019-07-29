using ITGlobal.MarkDocs.Source;
using JetBrains.Annotations;
using System;
using System.IO;

namespace ITGlobal.MarkDocs.Cache.Impl
{
    internal abstract class CachedAssetContainer<T>
        where T : Asset
    {
        private readonly IAssetStoreContext _ctx;

        protected CachedAssetContainer(IAssetStoreContext ctx)
        {
            _ctx = ctx;
        }

        private IMarkDocsLog Log => _ctx.Log;
        private ISourceTree SourceTree => _ctx.SourceTree;
        private string RootDirectory => _ctx.RootDirectory;
        private bool DisableCache => _ctx.DisableCache || OldIndex == null || OldDirectory == null;
        private CompilationEventListener EventListener => _ctx.EventListener;

        [CanBeNull]
        private string OldDirectory => _ctx.OldDirectory;
        private string NewDirectory => _ctx.NewDirectory;

        [CanBeNull]
        private DiskCacheIndex OldIndex => _ctx.OldIndex;
        private DiskCacheIndex NewIndex => _ctx.NewIndex;

        public void Store(T asset, Action<Stream> write)
        {
            var newHash = GetHashCode(asset);

            var newFileName = GetFileName(asset).Replace("\\", "/");
            var newFilePath = Path.Combine(NewDirectory, newFileName);
            Directory.CreateDirectory(Path.GetDirectoryName(newFilePath));

            var oldAssets = OldIndex != null ? SelectDictionary(OldIndex) : null;
            var newAssets = SelectDictionary(NewIndex);

            if (!DisableCache)
            {
                if (oldAssets != null && oldAssets.TryGetValue(asset.Id, out var oldAsset))
                {
                    if (string.Equals(newHash, oldAsset.Hash, StringComparison.OrdinalIgnoreCase))
                    {
                        var oldFilePath = Path.Combine(RootDirectory, OldDirectory, oldAsset.Filename);
                        try
                        {
                            SafeFileCopy(oldFilePath, newFilePath);
                            newAssets.Set(asset.Id, oldAsset.Filename, oldAsset.Hash);
                            Cached(asset);
                            return;
                        }
                        catch (Exception e)
                        {
                            Log.Error(e, $"Unable to copy asset '{oldFilePath}' into '{newFilePath}', will recreate it instead");
                        }
                    }
                }
            }

            using (var stream = File.OpenWrite(newFilePath))
            {
                write(stream);
            }

            newAssets.Set(asset.Id, newFileName, newHash);
            Written(asset);
        }

        public Stream ReadAsset(T asset)
        {
            if (!SelectDictionary(NewIndex).TryGetValue(asset.Id, out var item))
            {
                throw new Exception($"Asset \"{asset.Id}\" is not cached");
            }

            var newFilePath = Path.Combine(NewDirectory, item.Filename);
            return File.OpenRead(newFilePath);
        }

        public void CopyInheritedAssets()
        {
            if (OldIndex != null)
            {
                var oldAssets = SelectDictionary(OldIndex);
                var newAssets = SelectDictionary(NewIndex);
                foreach (var (id, oldItem) in oldAssets)
                {
                    if (newAssets.ContainsKey(id))
                    {
                        continue;
                    }

                    var oldFilePath = Path.Combine(RootDirectory, OldDirectory, oldItem.Filename);
                    var newFilePath = Path.Combine(RootDirectory, NewDirectory, oldItem.Filename);

                    Directory.CreateDirectory(Path.GetDirectoryName(newFilePath));
                    SafeFileCopy(oldFilePath, newFilePath);
                    newAssets.Set(id, oldItem.Filename, oldItem.Hash);
                    Cached(id);
                }
            }
        }

        protected abstract DiskCacheIndex.Dictionary SelectDictionary(DiskCacheIndex index);
        protected abstract string GetHashCode(T asset);
        protected abstract string GetFileName(T asset);

        protected virtual void Written(T asset) => EventListener.Written(asset);
        protected virtual void Cached(T asset) => EventListener.Cached(asset);
        protected virtual void Cached(string assetId) => EventListener.Cached(assetId);

        private void SafeFileCopy(string source, string destination)
        {
            const int attempts = 10;

            for (var i = 0; ; i++)
            {
                try
                {
                    File.Copy(source, destination);
                }
                catch (Exception e) when (i < attempts)
                {
                    Log.Error($"Unable to copy asset '{source}' into '{destination}', will retry ({e.Message})");
                }
            }
        }
    }
}