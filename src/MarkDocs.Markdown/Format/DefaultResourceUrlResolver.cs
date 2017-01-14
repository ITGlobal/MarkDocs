﻿namespace ITGlobal.MarkDocs.Format
{
    internal sealed class DefaultResourceUrlResolver : IResourceUrlResolver
    {
        public string ResolveUrl(IPage page, string resourceId)
        {
            var url = resourceId.StartsWith("/") ? $"/{resourceId}" : $"/{page.Id}/{resourceId}";
            return url;
        }
    }
}