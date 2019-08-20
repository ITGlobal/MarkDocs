namespace ITGlobal.MarkDocs.Site.Extensions
{
    public static class ResourceHelper
    {
        public static string GetResourceUrl(this IResourceId resource)
        {
            var url = Env.PUBLIC_URL + resource.Id;
            return url;
        }
    }
}