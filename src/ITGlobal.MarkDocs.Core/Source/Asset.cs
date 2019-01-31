namespace ITGlobal.MarkDocs.Source
{
    public abstract class Asset : IResourceId
    {
        protected Asset(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}
