namespace ITGlobal.MarkDocs.Content
{
    internal interface IIgnoreRule
    {
        bool ShouldIgnore(string path);
    }
}