namespace ITGlobal.MarkDocs.Source.Impl
{
    internal sealed class RemoteInfo
    {
        public RemoteInfo(string name, string hash)
        {
            Name = name;
            Hash = hash;
        }

        public string Name { get; }
        public string Hash { get; }
    }
}