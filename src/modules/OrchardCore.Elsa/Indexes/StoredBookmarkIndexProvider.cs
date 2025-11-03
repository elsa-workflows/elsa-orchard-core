using JetBrains.Annotations;
using OrchardCore.Elsa.Documents;
using YesSql.Indexes;

namespace OrchardCore.Elsa.Indexes;

[UsedImplicitly]
public class StoredBookmarkIndexProvider : IndexProvider<StoredBookmarkDocument>
{
    public StoredBookmarkIndexProvider()
    {
        CollectionName = ElsaCollections.StoredBookmarks;
    }

    public override void Describe(DescribeContext<StoredBookmarkDocument> context)
    {
        context.For<StoredBookmarkIndex>().Map(document => new()
        {
            BookmarkId = document.BookmarkId,
            WorkflowInstanceId = document.WorkflowInstanceId,
            CorrelationId = document.CorrelationId,
            ActivityInstanceId = document.ActivityInstanceId,
            Hash = document.Hash,
        });
    }
}