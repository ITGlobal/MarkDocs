namespace ITGlobal.MarkDocs.Source
{
    public interface IContentHashProvider
    {
        bool TryGetContentHash(string path, out string contentHash);
    }
}