using Elsa.Workflows.Runtime.Entities;
using YesSql.Indexes;

namespace OrchardCore.Elsa.Indexes;

public class WorkflowExecutionLogRecordIndexProvider : IndexProvider<WorkflowExecutionLogRecord>
{
    public WorkflowExecutionLogRecordIndexProvider()
    {
        CollectionName = ElsaCollections.WorkflowExecutionLogRecords;
    }
    
    public override void Describe(DescribeContext<WorkflowExecutionLogRecord> context)
    {
        context.For<WorkflowExecutionLogRecordIndex>().Map(record => new()
        {
            RecordId = record.Id,
            WorkflowInstanceId = record.WorkflowInstanceId,
            EventName = record.EventName,
            ActivityNodeId = record.ActivityNodeId,
            ParentActivityInstanceId = record.ParentActivityInstanceId,
            ActivityId = record.ActivityId,
            Timestamp = record.Timestamp,
            Sequence = record.Sequence,
        });
    }
}