using Elsa.Workflows.Runtime.Entities;
using YesSql.Indexes;

namespace OrchardCore.ElsaWorkflows.Indexes;

public class ActivityExecutionRecordIndexProvider : IndexProvider<ActivityExecutionRecord>
{
    public ActivityExecutionRecordIndexProvider()
    {
        CollectionName = ElsaCollections.ActivityExecutionRecords;
    }

    public override void Describe(DescribeContext<ActivityExecutionRecord> context)
    {
        context.For<ActivityExecutionRecordIndex>().Map(record => new()
        {
            RecordId = record.Id,
            WorkflowInstanceId = record.WorkflowInstanceId,
            ActivityNodeId = record.ActivityNodeId,
            ActivityId = record.ActivityId,
            ActivityType = record.ActivityType,
            ActivityTypeVersion = record.ActivityTypeVersion,
            StartedAt = record.StartedAt,
            HasBookmarks = record.HasBookmarks,
            CompletedAt = record.CompletedAt,
            Completed = record.CompletedAt != null,
            Status = record.Status,
            ActivityName = record.ActivityName
        });
    }
}