namespace ITGlobal.MarkDocs.Format.Icons
{
    internal sealed class IconType
    {
        public IconType(string type, string prefix)
        {
            Type = type;
            Prefix = prefix;
        }

        public IconType(string type)
        {
            Type = type;
            Prefix = type + "-";
        }

        public string Type { get; }
        public string Prefix { get; }
    }
}