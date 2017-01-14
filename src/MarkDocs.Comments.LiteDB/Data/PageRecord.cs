using LiteDB;

namespace ITGlobal.MarkDocs.Comments.Data
{
    /// <summary>
     ///     A page info
     /// </summary>
    internal sealed class PageRecord
    {
        public const string Collection = "pages";

        /// <summary>
        ///     ID
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; }

        /// <summary>
        ///     Documentation ID
        /// </summary>
        [BsonField("doc_id")]
        public string DocumentationId { get; set; }

        /// <summary>
        ///     Page ID
        /// </summary>
        [BsonField("page_id")]
        public string PageId { get; set; }

        public static LiteCollection<PageRecord> GetCollection(LiteDatabase db)
        {
            var collection = db.GetCollection<PageRecord>(Collection);
            collection.EnsureIndex(_ => _.DocumentationId);
            collection.EnsureIndex(_ => _.PageId);
            return collection;
        }

        public static PageRecord GetOrCreate(LiteDatabase db, string documentationId, string pageId)
        {
            var collection = GetCollection(db);
            var page = collection.FindOne(_ => _.DocumentationId == documentationId && _.PageId == pageId);
            if (page == null)
            {
                page = new PageRecord
                {
                    DocumentationId = documentationId,
                    PageId = pageId
                };
                collection.Insert(page);
            }

            return page;
        }

        public static PageRecord Get(LiteDatabase db, string documentationId, string pageId)
        {
            var collection = GetCollection(db);
            var page = collection.FindOne(_ => _.DocumentationId == documentationId && _.PageId == pageId);
            
            return page;
        }
    }
}
