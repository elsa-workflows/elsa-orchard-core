using Elsa.Workflows.Runtime.Entities;
using Elsa.Workflows.Runtime.Filters;
using OrchardCore.Elsa.Documents;
using OrchardCore.Elsa.Indexes;
using YesSql;

namespace OrchardCore.Elsa.Extensions;

public static class StoredBookmarkQueryExtensions
{
    public static IQuery<StoredBookmarkDocument, StoredBookmarkIndex> Apply(this IQuery<StoredBookmarkDocument, StoredBookmarkIndex> query, BookmarkFilter filter)
    {
        return filter.Apply(query);
    }
}