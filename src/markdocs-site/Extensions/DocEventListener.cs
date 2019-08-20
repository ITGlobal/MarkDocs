using System;
using ITGlobal.MarkDocs.Source;
using Serilog;

namespace ITGlobal.MarkDocs.Site.Extensions
{
    public sealed class DocEventListener : MarkDocsEventListener
    {
        public override CompilationEventListener CompilationStarted(string id)
        {
            return new ItgCompilationEventListener(id);
        }

        public override void SourceChanged()
        {
            Log.Information("Documentation source change detected");
        }

        public override void SourceChanged(ISourceTree sourceTree)
        {
            Log.Information("[{Doc}] source change detected", sourceTree.Id);
        }

        private sealed class ItgCompilationEventListener : CompilationEventListener
        {
            private readonly string _id;

            public ItgCompilationEventListener(string id)
            {
                _id = id;
            }

            public override void ReadingAssetTree()
            {
                Log.Debug("[{Doc}] reading assets", _id);
            }

            public override void ProcessingAssets(AssetTree tree)
            {
                Log.Debug("[{Doc}] processing assets", _id);
            }

            public override void Committing()
            {
                Log.Debug("[{Doc}] completing", _id);
            }

            public override void Completed(TimeSpan elapsed)
            {
                Log.Information("[{Doc}] compiled in {T:F1}s", _id, elapsed.TotalSeconds);
            }

            public override void Warning(string filename, string message, string location = null)
            {
                var src = location != null ? $"{filename}:{location}" : filename;
                Log.Warning("[{Doc}] {Where}: {What}", _id, src, message);
            }

            public override void Error(string message)
            {
                Log.Error("[{Doc}] {What}", _id, message);
            }

            public override void Error(string filename, string message, string location = null)
            {
                var src = location != null ? $"{filename}:{location}" : filename;
                Log.Error("[{Doc}] {Where}: {What}", _id, src, message);
            }

            public override void Written(Asset asset)
            {
                Log.Debug("[{Doc}] + {Id}", _id, asset.Id);
            }

            public override void Cached(Asset asset)
            {
                //Log.Debug("[{Doc}] ~ {Id}", _id, asset.Id);
            }

            public override void Cached(string assetId)
            {
                //Log.Debug("[{Doc}] ~ {Id}", _id, assetId);
            }
        }
    }
}