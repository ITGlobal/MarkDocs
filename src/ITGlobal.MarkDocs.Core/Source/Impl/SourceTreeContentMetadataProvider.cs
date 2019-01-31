using System.Collections.Generic;

namespace ITGlobal.MarkDocs.Source.Impl
{
    internal sealed class SourceTreeContentMetadataProvider : IContentMetadataProvider
    {
        public PageMetadata GetMetadata(
            ISourceTreeRoot sourceTreeRoot, 
            string filename,
            ICompilationReportBuilder report,
            HashSet<string> consumedFiles,
            bool isIndexFile)
        {
            var contentId = sourceTreeRoot.GetContentId(filename);
            var lastChangeAuthor = sourceTreeRoot.GetLastChangeAuthor( filename);

            return new PageMetadata(
                contentId: contentId,
                lastChangedBy: lastChangeAuthor
            );
        }
    }
}