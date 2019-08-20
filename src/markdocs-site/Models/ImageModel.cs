namespace ITGlobal.MarkDocs.Site.Models
{
    public sealed class ImageModel
    {
        public ImageModel(string url, string alt, string title)
        {
            Url = url;
            Alt = alt;
            Title = title;
        }

        public string Url { get; }
        public string Alt { get; }
        public string Title { get; }
    }
}