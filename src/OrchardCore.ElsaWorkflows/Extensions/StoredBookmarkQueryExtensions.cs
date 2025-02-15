using Elsa.Workflows.Runtime.Entities;
using Elsa.Workflows.Runtime.Filters;
using OrchardCore.ElsaWorkflows.Indexes;
using YesSql;

namespace OrchardCore.ElsaWorkflows.Extensions;

public static class StoredBookmarkQueryExtensions
{
    public static IQuery<StoredBookmark, StoredBookmarkIndex> Apply(this IQuery<StoredBookmark, StoredBookmarkIndex> query, BookmarkFilter filter)
    {
        return filter.Apply(query);
    }
}