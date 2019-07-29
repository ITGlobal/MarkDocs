using System;
using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Impl
{
    internal sealed class DefaultCompilationEventListener : CompilationEventListener
    {
        private readonly IMarkDocsLog _log;
        private readonly string _id;

        public DefaultCompilationEventListener(IMarkDocsLog log, string id)
        {
            _log = log;
            _id = id;
        }

        public override void ReadingAssetTree()
        {
          //  _log.Debug($"[{_id}]: reading asset tree");
        }

        public override void ProcessingAssets(AssetTree tree)
        {
          //  _log.Debug($"[{_id}]: processing assets");
        }

        public override void Cached(Asset asset)
        {
           // _log.Info($"[{_id}]: cached {asset.Id}");
        }

        public override void Cached(string assetId)
        {
           // _log.Info($"[{_id}]: cached {assetId}");
        }

        public override void Written(Asset asset)
        {
            //_log.Info($"[{_id}]: written {asset.Id}");
        }

        public override void Committing()
        {
            //_log.Info($"[{_id}]: flushing...");
        }

        public override void Completed(TimeSpan elapsed)
        {
            _log.Info($"[{_id}]: compiled in {elapsed.TotalSeconds:F1}s");
        }
    }
}