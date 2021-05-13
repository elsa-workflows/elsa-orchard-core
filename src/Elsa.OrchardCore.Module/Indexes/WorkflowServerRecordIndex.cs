using Elsa.OrchardCore.Models;
using YesSql.Indexes;

namespace Elsa.OrchardCore.Indexes
{
    public class WorkflowServerRecordIndex : MapIndex
    {
        public string WorkflowServerId { get; set; } = default!;
    }

    public class RemoteWorkflowServerIndexProvider : IndexProvider<RemoteWorkflowServerRecord>
    {
        public override void Describe(DescribeContext<RemoteWorkflowServerRecord> context)
        {
            context.For<WorkflowServerRecordIndex>()
                .Map(workflowServer =>
                    new WorkflowServerRecordIndex
                    {
                        WorkflowServerId = workflowServer.WorkflowServerId,
                    }
                );
        }
    }
}