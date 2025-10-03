using Elsa.Workflows.Runtime.Entities;
using YesSql.Indexes;

namespace OrchardCore.Elsa.Indexes;

public class StoredBookmarkIndexProvider : IndexProvider<StoredBookmark>
{
    public StoredBookmarkIndexProvider()
    {
        CollectionName = ElsaCollections.StoredBookmarks;
    }
    
    public override void Describe(DescribeContext<StoredBookmark> context)
    {
        context.For<StoredBookmarkIndex>().Map(bookmark => new()
        {
            BookmarkId = bookmark.Id,
            WorkflowInstanceId = bookmark.WorkflowInstanceId,
            CorrelationId = bookmark.CorrelationId,
            ActivityTypeName = bookmark.ActivityTypeName,
            ActivityInstanceId = bookmark.ActivityInstanceId,
            Hash = bookmark.Hash,
        });
    }
}