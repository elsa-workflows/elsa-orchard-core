using JetBrains.Annotations;
using OrchardCore.Elsa.Documents;
using YesSql.Indexes;

namespace OrchardCore.Elsa.Indexes;

[UsedImplicitly]
public class WorkflowExecutionLogRecordIndexProvider : IndexProvider<WorkflowExecutionLogRecordDocument>
{
    public WorkflowExecutionLogRecordIndexProvider()
    {
        CollectionName = ElsaCollections.WorkflowExecutionLogRecords;
    }

    public override void Describe(DescribeContext<WorkflowExecutionLogRecordDocument> context)
    {
        context.For<WorkflowExecutionLogRecordIndex>().Map(document => new()
        {
            RecordId = document.RecordId,
            WorkflowInstanceId = document.WorkflowInstanceId,
            EventName = document.EventName,
            ActivityNodeId = document.ActivityNodeId,
            ParentActivityInstanceId = document.ParentActivityInstanceId,
            ActivityId = document.ActivityId,
            Timestamp = document.Timestamp,
            Sequence = document.Sequence,
        });
    }
}