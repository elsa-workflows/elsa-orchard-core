using Elsa.Common.Entities;
using Elsa.Workflows.Runtime.Entities;
using Elsa.Workflows.Runtime.Filters;
using Elsa.Workflows.Runtime.OrderDefinitions;
using OrchardCore.Elsa.Indexes;
using YesSql;

namespace OrchardCore.Elsa.Extensions;

public static class WorkflowExecutionLogRecordQueryExtensions
{
    public static IQuery<WorkflowExecutionLogRecord, WorkflowExecutionLogRecordIndex> Apply(this IQuery<WorkflowExecutionLogRecord, WorkflowExecutionLogRecordIndex> query, WorkflowExecutionLogRecordFilter filter)
    {
        return filter.Apply(query);
    }
    
    public static IQuery<WorkflowExecutionLogRecord, WorkflowExecutionLogRecordIndex> Apply<TOrderBy>(this IQuery<WorkflowExecutionLogRecord, WorkflowExecutionLogRecordIndex> query, WorkflowExecutionLogRecordOrder<TOrderBy> order)
    {
        var keySelector = ExpressionConverter.Convert<WorkflowExecutionLogRecord, WorkflowExecutionLogRecordIndex, TOrderBy>(order.KeySelector);
        return order.Direction == OrderDirection.Ascending 
            ? query.OrderBy(keySelector) 
            : query.OrderByDescending(keySelector);
    }
}