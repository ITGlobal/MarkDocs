namespace ITGlobal.MarkDocs.Tools.Serve
{
    public interface IServerConfig
    {
        string ListenUrl { get; }
        string PublicUrl { get; }
        bool EnableDeveloperMode { get; }
        bool Verbose { get; }
        bool Quiet { get; }
        Theme Theme { get; }

        void Configure(MarkDocsOptions config);
    }
}