using Elsa.Workflows.Runtime.Entities;
using Elsa.Workflows.Runtime.Filters;
using OrchardCore.ElsaWorkflows.Indexes;
using YesSql;

namespace OrchardCore.ElsaWorkflows.Extensions;

public static class StoredTriggerQueryExtensions
{
    public static IQuery<StoredTrigger, StoredTriggerIndex> Apply(this IQuery<StoredTrigger, StoredTriggerIndex> query, TriggerFilter filter)
    {
        return filter.Apply(query);
    }
}