namespace ITGlobal.MarkDocs.Tools.Serve.Models
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
