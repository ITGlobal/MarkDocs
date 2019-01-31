namespace ITGlobal.MarkDocs.Source.Impl
{
    internal interface IIgnoreRule
    {
        bool ShouldIgnore(string path);
    }
}