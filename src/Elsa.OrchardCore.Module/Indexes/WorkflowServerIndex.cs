using Elsa.OrchardCore.Models;
using YesSql.Indexes;

namespace Elsa.OrchardCore.Indexes
{
    public class WorkflowServerIndex : MapIndex
    {
        public string WorkflowServerId { get; set; } = default!;
    }

    public class WorkflowServerIndexProvider : IndexProvider<WorkflowServer>
    {
        public override void Describe(DescribeContext<WorkflowServer> context)
        {
            context.For<WorkflowServerIndex>()
                .Map(workflowServer =>
                    new WorkflowServerIndex
                    {
                        WorkflowServerId = workflowServer.WorkflowServerId,
                    }
                );
        }
    }
}