using Elsa.Workflows.Runtime.Entities;
using Elsa.Workflows.Runtime.Filters;
using OrchardCore.ElsaWorkflows.Indexes;
using YesSql;
using YesSql.Services;

namespace OrchardCore.ElsaWorkflows.Extensions;

public static class StoredBookmarkFilterExtensions
{
    public static IQuery<StoredBookmark, StoredBookmarkIndex> Apply(this BookmarkFilter filter, IQuery<StoredBookmark, StoredBookmarkIndex> query)
    {
        if (filter.BookmarkId != null) query = query.Where(x => x.BookmarkId == filter.BookmarkId);
        if (filter.BookmarkIds != null) query = query.Where(x => x.BookmarkId.IsIn(filter.BookmarkIds));
        if (filter.CorrelationId != null) query = query.Where(x => x.CorrelationId == filter.CorrelationId);
        if (filter.Hash != null) query = query.Where(x => x.Hash == filter.Hash);
        if (filter.Hashes != null) query = query.Where(x => x.Hash.IsIn(filter.Hashes));
        if (filter.WorkflowInstanceId != null) query = query.Where(x => x.WorkflowInstanceId == filter.WorkflowInstanceId);
        if (filter.WorkflowInstanceIds != null) query = query.Where(x => x.WorkflowInstanceId.IsIn(filter.WorkflowInstanceIds));
        if (filter.Name != null) query = query.Where(x => x.ActivityTypeName == filter.Name);
        if (filter.Names != null) query = query.Where(x => x.ActivityTypeName.IsIn(filter.Names));
        if (filter.ActivityInstanceId != null) query = query.Where(x => x.ActivityInstanceId == filter.ActivityInstanceId);
        
        return query;
    }
}