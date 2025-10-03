using Elsa.Workflows.Runtime.Entities;
using Elsa.Workflows.Runtime.Filters;
using OrchardCore.Elsa.Indexes;
using YesSql;

namespace OrchardCore.Elsa.Extensions;

public static class StoredBookmarkQueryExtensions
{
    public static IQuery<StoredBookmark, StoredBookmarkIndex> Apply(this IQuery<StoredBookmark, StoredBookmarkIndex> query, BookmarkFilter filter)
    {
        return filter.Apply(query);
    }
}