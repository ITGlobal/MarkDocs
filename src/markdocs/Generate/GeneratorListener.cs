using System;
using ITGlobal.MarkDocs.Source;
using Serilog;

namespace ITGlobal.MarkDocs.Tools.Generate
{
    public sealed class GeneratorListener : MarkDocsEventListener
    {
        private readonly ITerminalOutput _output;

        public GeneratorListener(ITerminalOutput output)
        {
            _output = output;
        }

        public override CompilationEventListener CompilationStarted(string id)
           => new GeneratorCompilationEventListener(_output);

        private sealed class GeneratorCompilationEventListener : CompilationEventListener
        {
            private readonly ITerminalOutput _output;
            private int _processedAssetCount;
            private int _assetCount;
            
            public GeneratorCompilationEventListener(ITerminalOutput output)
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