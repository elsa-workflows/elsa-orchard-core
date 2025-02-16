using Elsa.Workflows.Runtime.Entities;
using Elsa.Workflows.Runtime.Filters;
using OrchardCore.ElsaWorkflows.Indexes;
using YesSql;
using YesSql.Services;

namespace OrchardCore.ElsaWorkflows.Extensions;

public static class WorkflowExecutionLogRecordFilterExtensions
{
    public static IQuery<WorkflowExecutionLogRecord, WorkflowExecutionLogRecordIndex> Apply(this WorkflowExecutionLogRecordFilter filter, IQuery<WorkflowExecutionLogRecord, WorkflowExecutionLogRecordIndex> query)
    {
        if (filter.Id != null) query = query.Where(x => x.WorkflowExecutionLogRecordId == filter.Id);
        if (filter.Ids != null) query = query.Where(x => x.WorkflowExecutionLogRecordId.IsIn(filter.Ids));
        if (filter.WorkflowInstanceId != null) query = query.Where(x => x.WorkflowInstanceId == filter.WorkflowInstanceId);
        if (filter.WorkflowInstanceIds != null) query = query.Where(x => x.WorkflowExecutionLogRecordId.IsIn(filter.WorkflowInstanceIds));
        if (filter.ParentActivityInstanceId != null) query = query.Where(x => x.ParentActivityInstanceId == filter.ParentActivityInstanceId);
        if (filter.ActivityId != null) query = query.Where(x => x.ActivityId == filter.ActivityId);
        if (filter.ActivityIds != null) query = query.Where(x => x.ActivityId.IsIn(filter.ActivityIds));
        if (filter.ActivityNodeId != null) query = query.Where(x => x.ActivityNodeId == filter.ActivityNodeId);
        if (filter.ActivityNodeIds != null) query = query.Where(x => x.ActivityNodeId.IsIn(filter.ActivityNodeIds));
        if (filter.EventName != null) query = query.Where(x => x.EventName == filter.EventName);
        if (filter.EventNames != null) query = query.Where(x => x.EventName.IsIn(filter.EventNames));
        return query;
    }
}