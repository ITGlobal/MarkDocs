namespace ITGlobal.MarkDocs.Site.Models
{
    public sealed class ImageGalleryModel
    {
        public ImageGalleryModel(ImageModel[] images)
        {
            Images = images;
        }

        public ImageModel[] Images { get; }
    }
}
