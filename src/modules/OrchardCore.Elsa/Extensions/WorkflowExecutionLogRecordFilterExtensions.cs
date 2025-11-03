using Elsa.Workflows.Runtime.Entities;
using Elsa.Workflows.Runtime.Filters;
using OrchardCore.Elsa.Documents;
using OrchardCore.Elsa.Indexes;
using YesSql;
using YesSql.Services;

namespace OrchardCore.Elsa.Extensions;

public static class WorkflowExecutionLogRecordFilterExtensions
{
    public static IQuery<WorkflowExecutionLogRecordDocument, WorkflowExecutionLogRecordIndex> Apply(this WorkflowExecutionLogRecordFilter filter, IQuery<WorkflowExecutionLogRecordDocument, WorkflowExecutionLogRecordIndex> query)
    {
        if (filter.Id != null) query = query.Where(x => x.RecordId == filter.Id);
        if (filter.Ids != null) query = query.Where(x => x.RecordId.IsIn(filter.Ids));
        if (filter.WorkflowInstanceId != null) query = query.Where(x => x.WorkflowInstanceId == filter.WorkflowInstanceId);
        if (filter.WorkflowInstanceIds != null) query = query.Where(x => x.RecordId.IsIn(filter.WorkflowInstanceIds));
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