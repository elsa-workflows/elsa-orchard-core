using Elsa.Workflows.Management.Entities;
using Elsa.Workflows.Management.Enums;
using Elsa.Workflows.Management.Filters;
using OrchardCore.Elsa.Indexes;
using YesSql;
using YesSql.Services;

namespace OrchardCore.Elsa.Extensions;

public static class WorkflowInstanceFilterExtensions
{
    public static IQuery<WorkflowInstance, WorkflowInstanceIndex> Apply(this WorkflowInstanceFilter filter, IQuery<WorkflowInstance, WorkflowInstanceIndex> query)
    {
        if (filter.Id != null) query = query.Where(x => x.InstanceId == filter.Id);
        if (filter.Ids != null) query = query.Where(x => x.InstanceId.IsIn(filter.Ids));
        if (filter.DefinitionId != null) query = query.Where(x => x.DefinitionId == filter.DefinitionId);
        if (filter.DefinitionIds != null) query = query.Where(x => x.DefinitionId.IsIn(filter.DefinitionIds));
        if (filter.DefinitionVersionId != null) query = query.Where(x => x.DefinitionId == filter.DefinitionVersionId);
        if (filter.DefinitionVersionIds != null) query = query.Where(x => x.DefinitionId.IsIn(filter.DefinitionVersionIds));
        if (filter.CorrelationId != null) query = query.Where(x => x.CorrelationId == filter.CorrelationId);
        if (filter.CorrelationIds != null) query = query.Where(x => x.CorrelationId.IsIn(filter.CorrelationIds));
        if (filter.Name != null) query = query.Where(x => x.Name == filter.Name);
        if (filter.WorkflowStatus != null) query = query.Where(x => x.Status == filter.WorkflowStatus);
        if (filter.WorkflowStatuses != null) query = query.Where(x => x.Status.IsIn(filter.WorkflowStatuses));
        if (filter.WorkflowSubStatus != null) query = query.Where(x => x.SubStatus == filter.WorkflowSubStatus);
        if (filter.WorkflowSubStatuses != null) query = query.Where(x => x.SubStatus.IsIn(filter.WorkflowSubStatuses));
        if (filter.Version != null) query = query.Where(x => x.Version == filter.Version);
        if (filter.ParentWorkflowInstanceIds != null) query = query.Where(x => x.ParentInstanceId.IsIn(filter.ParentWorkflowInstanceIds));
        if (filter.HasIncidents != null) query = query.Where(x => x.HasIncidents == filter.HasIncidents);
        if (filter.IsSystem != null) query = query.Where(x => x.IsSystem == filter.IsSystem);
        
        if (filter.TimestampFilters != null)
        {
            foreach (var timestampFilter in filter.TimestampFilters)
            {
                var column = timestampFilter.Column;
                var timestamp = timestampFilter.Timestamp;
                var isZeroTime = timestamp.TimeOfDay == TimeSpan.Zero;
                var startDay = new DateTimeOffset(timestamp.Date);
                var endDay = startDay.AddDays(1);

                query = timestampFilter.Operator switch
                {
                    TimestampFilterOperator.Is when isZeroTime => query.Where(dialect => $"{column} >= '{dialect.GetSqlValue(startDay)}' && {column} < '${dialect.GetSqlValue(endDay)}'"),
                    TimestampFilterOperator.Is => query.Where(dialect => $"{column} == '{dialect.GetSqlValue(timestamp)}'"),
                    TimestampFilterOperator.IsNot when isZeroTime => query.Where(dialect => $"{column} < '{dialect.GetSqlValue(startDay)}' || {column} >= {dialect.GetSqlValue(endDay)}"),
                    TimestampFilterOperator.IsNot => query.Where(dialect => $"{column} != '{dialect.GetSqlValue(timestamp)}'"),
                    TimestampFilterOperator.GreaterThan when isZeroTime => query.Where(dialect => $"{column} > '{dialect.GetSqlValue(endDay)}'"),
                    TimestampFilterOperator.GreaterThan => query.Where(dialect => $"{column} > '{dialect.GetSqlValue(timestamp)}'"),
                    TimestampFilterOperator.GreaterThanOrEqual when isZeroTime => query.Where(dialect => $"{column} >= '{dialect.GetSqlValue(startDay)}'"),
                    TimestampFilterOperator.GreaterThanOrEqual => query.Where(dialect => $"{column} >= '{dialect.GetSqlValue(timestamp)}'"),
                    TimestampFilterOperator.LessThan when isZeroTime => query.Where(dialect => $"{column} < '{dialect.GetSqlValue(endDay)}'"),
                    TimestampFilterOperator.LessThan => query.Where(dialect => $"{column} < '{dialect.GetSqlValue(timestamp)}'"),
                    TimestampFilterOperator.LessThanOrEqual when isZeroTime => query.Where(dialect => $"{column} <= '{dialect.GetSqlValue(endDay)}'"),
                    TimestampFilterOperator.LessThanOrEqual => query.Where(dialect => $"{column} <= '{dialect.GetSqlValue(timestamp)}'"),
                    _ => query
                };
            }
        }

        var searchTerm = filter.SearchTerm;
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query =
                from instance in query
                where instance.Name!.ToLower().Contains(searchTerm.ToLower())
                      || instance.DefinitionVersionId.Contains(searchTerm)
                      || instance.DefinitionId.Contains(searchTerm)
                      || instance.InstanceId.Contains(searchTerm)
                      || instance.CorrelationId!.Contains(searchTerm)
                select instance;
        }

        return query;
    }
    
    public static IQueryIndex<WorkflowInstanceIndex> Apply(this WorkflowInstanceFilter filter, IQueryIndex<WorkflowInstanceIndex> query)
    {
        if (filter.Id != null) query = query.Where(x => x.InstanceId == filter.Id);
        if (filter.Ids != null) query = query.Where(x => x.InstanceId.IsIn(filter.Ids));
        if (filter.DefinitionId != null) query = query.Where(x => x.DefinitionId == filter.DefinitionId);
        if (filter.DefinitionIds != null) query = query.Where(x => x.DefinitionId.IsIn(filter.DefinitionIds));
        if (filter.DefinitionVersionId != null) query = query.Where(x => x.DefinitionVersionId == filter.DefinitionVersionId);
        if (filter.DefinitionVersionIds != null) query = query.Where(x => x.DefinitionVersionId.IsIn(filter.DefinitionVersionIds));
        if (filter.CorrelationId != null) query = query.Where(x => x.CorrelationId == filter.CorrelationId);
        if (filter.CorrelationIds != null) query = query.Where(x => x.CorrelationId.IsIn(filter.CorrelationIds));
        if (filter.Name != null) query = query.Where(x => x.Name == filter.Name);
        if (filter.WorkflowStatus != null) query = query.Where(x => x.Status == filter.WorkflowStatus);
        if (filter.WorkflowStatuses != null) query = query.Where(x => x.Status.IsIn(filter.WorkflowStatuses));
        if (filter.WorkflowSubStatus != null) query = query.Where(x => x.SubStatus == filter.WorkflowSubStatus);
        if (filter.WorkflowSubStatuses != null) query = query.Where(x => x.SubStatus.IsIn(filter.WorkflowSubStatuses));
        if (filter.Version != null) query = query.Where(x => x.Version == filter.Version);
        if (filter.ParentWorkflowInstanceIds != null) query = query.Where(x => x.ParentInstanceId.IsIn(filter.ParentWorkflowInstanceIds));
        if (filter.HasIncidents != null) query = query.Where(x => x.HasIncidents == filter.HasIncidents);
        if (filter.IsSystem != null) query = query.Where(x => x.IsSystem == filter.IsSystem);
        
        if (filter.TimestampFilters != null)
        {
            foreach (var timestampFilter in filter.TimestampFilters)
            {
                var column = timestampFilter.Column;
                var timestamp = timestampFilter.Timestamp;
                var isZeroTime = timestamp.TimeOfDay == TimeSpan.Zero;
                var startDay = new DateTimeOffset(timestamp.Date);
                var endDay = startDay.AddDays(1);

                query = timestampFilter.Operator switch
                {
                    TimestampFilterOperator.Is when isZeroTime => query.Where(dialect => $"{column} >= '{dialect.GetSqlValue(startDay)}' && {column} < '${dialect.GetSqlValue(endDay)}'"),
                    TimestampFilterOperator.Is => query.Where(dialect => $"{column} == '{dialect.GetSqlValue(timestamp)}'"),
                    TimestampFilterOperator.IsNot when isZeroTime => query.Where(dialect => $"{column} < '{dialect.GetSqlValue(startDay)}' || {column} >= {dialect.GetSqlValue(endDay)}"),
                    TimestampFilterOperator.IsNot => query.Where(dialect => $"{column} != '{dialect.GetSqlValue(timestamp)}'"),
                    TimestampFilterOperator.GreaterThan when isZeroTime => query.Where(dialect => $"{column} > '{dialect.GetSqlValue(endDay)}'"),
                    TimestampFilterOperator.GreaterThan => query.Where(dialect => $"{column} > '{dialect.GetSqlValue(timestamp)}'"),
                    TimestampFilterOperator.GreaterThanOrEqual when isZeroTime => query.Where(dialect => $"{column} >= '{dialect.GetSqlValue(startDay)}'"),
                    TimestampFilterOperator.GreaterThanOrEqual => query.Where(dialect => $"{column} >= '{dialect.GetSqlValue(timestamp)}'"),
                    TimestampFilterOperator.LessThan when isZeroTime => query.Where(dialect => $"{column} < '{dialect.GetSqlValue(endDay)}'"),
                    TimestampFilterOperator.LessThan => query.Where(dialect => $"{column} < '{dialect.GetSqlValue(timestamp)}'"),
                    TimestampFilterOperator.LessThanOrEqual when isZeroTime => query.Where(dialect => $"{column} <= '{dialect.GetSqlValue(endDay)}'"),
                    TimestampFilterOperator.LessThanOrEqual => query.Where(dialect => $"{column} <= '{dialect.GetSqlValue(timestamp)}'"),
                    _ => query
                };
            }
        }

        var searchTerm = filter.SearchTerm;
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query =
                from instance in query
                where instance.Name!.ToLower().Contains(searchTerm.ToLower())
                      || instance.DefinitionVersionId.Contains(searchTerm)
                      || instance.DefinitionId.Contains(searchTerm)
                      || instance.InstanceId.Contains(searchTerm)
                      || instance.CorrelationId!.Contains(searchTerm)
                select instance;
        }

        return query;
    }
}