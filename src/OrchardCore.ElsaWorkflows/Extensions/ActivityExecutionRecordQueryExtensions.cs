using Elsa.Common.Entities;
using Elsa.Workflows.Runtime.Entities;
using Elsa.Workflows.Runtime.Filters;
using Elsa.Workflows.Runtime.OrderDefinitions;
using OrchardCore.ElsaWorkflows.Indexes;
using YesSql;

namespace OrchardCore.ElsaWorkflows.Extensions;

public static class ActivityExecutionRecordQueryExtensions
{
    public static IQuery<ActivityExecutionRecord, ActivityExecutionRecordIndex> Apply(this IQuery<ActivityExecutionRecord, ActivityExecutionRecordIndex> query, ActivityExecutionRecordFilter filter)
    {
        return filter.Apply(query);
    }
    
    public static IQuery<ActivityExecutionRecord, ActivityExecutionRecordIndex> Apply<TOrderBy>(this IQuery<ActivityExecutionRecord, ActivityExecutionRecordIndex> query, ActivityExecutionRecordOrder<TOrderBy> order)
    {
        var keySelector = ExpressionConverter.Convert<ActivityExecutionRecord, ActivityExecutionRecordIndex, TOrderBy>(order.KeySelector);
        return order.Direction == OrderDirection.Ascending 
            ? query.OrderBy(keySelector) 
            : query.OrderByDescending(keySelector);
    }
    
    public static IQueryIndex<ActivityExecutionRecordIndex> Apply(this IQueryIndex<ActivityExecutionRecordIndex> query, ActivityExecutionRecordFilter filter)
    {
        return filter.Apply(query);
    }
    
    public static IQueryIndex<ActivityExecutionRecordIndex> Apply<TOrderBy>(this IQueryIndex<ActivityExecutionRecordIndex> query, ActivityExecutionRecordOrder<TOrderBy> order)
    {
        var keySelector = ExpressionConverter.Convert<ActivityExecutionRecord, ActivityExecutionRecordIndex, TOrderBy>(order.KeySelector);
        return order.Direction == OrderDirection.Ascending 
            ? query.OrderBy(keySelector) 
            : query.OrderByDescending(keySelector);
    }
}