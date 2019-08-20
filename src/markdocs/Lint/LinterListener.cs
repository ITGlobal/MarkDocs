using ITGlobal.MarkDocs.Source;
using System;
using Serilog;

namespace ITGlobal.MarkDocs.Tools.Lint
{
    public sealed class LinterListener : MarkDocsEventListener
    {
        private readonly ITerminalOutput _output;

        public LinterListener(ITerminalOutput output)
        {
            _output = output;
        }

        public override CompilationEventListener CompilationStarted(string id)
            => new LinterCompilationEventListener(_output);

        private sealed class LinterCompilationEventListener : CompilationEventListener
        {
            private readonly ITerminalOutput _output;

            private int _processedAssetCount;
            private int _assetCount;

            public LinterCompilationEventListener(ITerminalOutput output)
            {
                _output = output;
            }

            public override void ReadingAssetTree()
            {
                _output.Write(0, "reading assets");
            }

            public override void ProcessingAssets(AssetTree tree)
            {
                _assetCount = tree.Pages.Count;
                _output.Write(0, "compiling assets");
            }

            public override void Cached(Asset asset)
            {
                switch (asset)
                {
                    case PageAsset _:
                        _processedAssetCount++;
                        var progress = (int)Math.Ceiling(100f * _processedAssetCount / _assetCount);
                        _output.Write(progress);
                        break;
                }

                Log.Information("Processed {Id}", asset.Id);
            }

            public override void Cached(string assetId)
            {
                Log.Information("Processed {Id}", assetId);
            }

            public override void Written(Asset asset)
            {
                switch (asset)
                {
                    case PageAsset _:
                        _processedAssetCount++;
                        var progress = (int)Math.Ceiling(100f * _processedAssetCount / _assetCount);
                        _output.Write(progress);
                        break;
                }

                Log.Information("Processed {Id}", asset.Id);
            }

            public override void Committing() { }

            public override void Completed(TimeSpan elapsed)
            {
                Log.Information("Completed in {T:F1}s", elapsed.TotalSeconds);
                _output.Write(100);
            }
        }
    }
}