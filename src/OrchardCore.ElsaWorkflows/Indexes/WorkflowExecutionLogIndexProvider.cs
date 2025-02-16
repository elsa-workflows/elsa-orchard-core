using Elsa.Workflows.Runtime.Entities;
using YesSql.Indexes;

namespace OrchardCore.ElsaWorkflows.Indexes;

public class WorkflowExecutionLogIndexProvider : IndexProvider<WorkflowExecutionLogRecord>
{
    public WorkflowExecutionLogIndexProvider()
    {
        CollectionName = ElsaCollections.WorkflowExecutionLogs;
    }
    
    public override void Describe(DescribeContext<WorkflowExecutionLogRecord> context)
    {
        context.For<WorkflowExecutionLogRecordIndex>().Map(record => new()
        {
            WorkflowInstanceId = record.WorkflowInstanceId
        });
    }
}