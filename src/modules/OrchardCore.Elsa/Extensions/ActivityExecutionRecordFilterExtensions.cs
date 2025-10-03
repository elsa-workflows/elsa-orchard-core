using Elsa.Workflows.Runtime.Entities;
using Elsa.Workflows.Runtime.Filters;
using OrchardCore.Elsa.Indexes;
using YesSql;
using YesSql.Services;

namespace OrchardCore.Elsa.Extensions;

public static class ActivityExecutionRecordFilterExtensions
{
    public static IQuery<ActivityExecutionRecord, ActivityExecutionRecordIndex> Apply(this ActivityExecutionRecordFilter filter, IQuery<ActivityExecutionRecord, ActivityExecutionRecordIndex> query)
    {
        if (filter.Id != null) query = query.Where(x => x.RecordId == filter.Id);
        if (filter.Ids != null) query = query.Where(x => x.RecordId.IsIn(filter.Ids));
        if (filter.WorkflowInstanceId != null) query = query.Where(x => x.WorkflowInstanceId == filter.WorkflowInstanceId);
        if (filter.WorkflowInstanceIds != null) query = query.Where(x => x.RecordId.IsIn(filter.WorkflowInstanceIds));
        if (filter.ActivityId != null) query = query.Where(x => x.ActivityId == filter.ActivityId);
        if (filter.ActivityIds != null) query = query.Where(x => x.ActivityId.IsIn(filter.ActivityIds));
        if (filter.ActivityNodeId != null) query = query.Where(x => x.ActivityNodeId == filter.ActivityNodeId);
        if (filter.ActivityNodeIds != null) query = query.Where(x => x.ActivityNodeId.IsIn(filter.ActivityNodeIds));
        if (filter.Name != null) query = query.Where(x => x.ActivityName == filter.Name);
        if (filter.Names != null) query = query.Where(x => x.ActivityName.IsIn(filter.Names));
        if (filter.Status != null) query = query.Where(x => x.Status == filter.Status);
        if (filter.Statuses != null) query = query.Where(x => x.Status.IsIn(filter.Statuses));
        if (filter.Completed != null) query = query.Where(x => x.Completed);
        return query;
    }
    
    public static IQueryIndex<ActivityExecutionRecordIndex> Apply(this ActivityExecutionRecordFilter filter, IQueryIndex<ActivityExecutionRecordIndex> query)
    {
        if (filter.Id != null) query = query.Where(x => x.RecordId == filter.Id);
        if (filter.Ids != null) query = query.Where(x => x.RecordId.IsIn(filter.Ids));
        if (filter.WorkflowInstanceId != null) query = query.Where(x => x.WorkflowInstanceId == filter.WorkflowInstanceId);
        if (filter.WorkflowInstanceIds != null) query = query.Where(x => x.RecordId.IsIn(filter.WorkflowInstanceIds));
        if (filter.ActivityId != null) query = query.Where(x => x.ActivityId == filter.ActivityId);
        if (filter.ActivityIds != null) query = query.Where(x => x.ActivityId.IsIn(filter.ActivityIds));
        if (filter.ActivityNodeId != null) query = query.Where(x => x.ActivityNodeId == filter.ActivityNodeId);
        if (filter.ActivityNodeIds != null) query = query.Where(x => x.ActivityNodeId.IsIn(filter.ActivityNodeIds));
        if (filter.Name != null) query = query.Where(x => x.ActivityName == filter.Name);
        if (filter.Names != null) query = query.Where(x => x.ActivityName.IsIn(filter.Names));
        if (filter.Status != null) query = query.Where(x => x.Status == filter.Status);
        if (filter.Statuses != null) query = query.Where(x => x.Status.IsIn(filter.Statuses));
        if (filter.Completed != null) query = query.Where(x => x.Completed);
        return query;
    }
}