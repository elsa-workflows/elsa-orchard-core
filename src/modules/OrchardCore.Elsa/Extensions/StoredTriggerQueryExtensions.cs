using Elsa.Common.Entities;
using Elsa.Workflows.Runtime.Entities;
using Elsa.Workflows.Runtime.Filters;
using Elsa.Workflows.Runtime.OrderDefinitions;
using OrchardCore.Elsa.Indexes;
using YesSql;

namespace OrchardCore.Elsa.Extensions;

public static class StoredTriggerQueryExtensions
{
    public static IQuery<StoredTrigger, StoredTriggerIndex> Apply(this IQuery<StoredTrigger, StoredTriggerIndex> query, TriggerFilter filter)
    {
        return filter.Apply(query);
    }
    
    public static IQueryIndex<StoredTriggerIndex> Apply(this IQueryIndex<StoredTriggerIndex> query, TriggerFilter filter)
    {
        return filter.Apply(query);
    }
    
    public static IQuery<StoredTrigger, StoredTriggerIndex> Apply<TOrderBy>(this IQuery<StoredTrigger, StoredTriggerIndex> query, StoredTriggerOrder<TOrderBy> order)
    {
        var keySelector = ExpressionConverter.Convert<StoredTrigger, StoredTriggerIndex, TOrderBy>(order.KeySelector);
        return order.Direction == OrderDirection.Ascending 
            ? query.OrderBy(keySelector) 
            : query.OrderByDescending(keySelector);
    }
    
    public static IQueryIndex<StoredTriggerIndex> Apply<TOrderBy>(this IQueryIndex<StoredTriggerIndex> query, StoredTriggerOrder<TOrderBy> order)
    {
        var keySelector = ExpressionConverter.Convert<StoredTrigger, StoredTriggerIndex, TOrderBy>(order.KeySelector);
        return order.Direction == OrderDirection.Ascending 
            ? query.OrderBy(keySelector) 
            : query.OrderByDescending(keySelector);
    }
}