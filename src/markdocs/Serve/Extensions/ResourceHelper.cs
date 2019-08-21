namespace ITGlobal.MarkDocs.Tools.Serve.Extensions
{
    public static class ResourceHelper
    {
        public static string GetResourceUrl(this IResourceId resource)
        {
            var url = Startup.Config.PublicUrl + resource.Id;
            return url;
        }
    }
}