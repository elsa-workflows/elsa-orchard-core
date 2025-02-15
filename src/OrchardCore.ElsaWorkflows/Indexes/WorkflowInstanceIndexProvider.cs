using Elsa.Workflows.Management.Entities;
using YesSql.Indexes;

namespace OrchardCore.ElsaWorkflows.Indexes;

public class WorkflowInstanceIndexProvider : IndexProvider<WorkflowInstance>
{
    public override void Describe(DescribeContext<WorkflowInstance> context)
    {
        context.For<WorkflowInstanceIndex>().Map(instance => new()
        {
            InstanceId = instance.Id,
            DefinitionId = instance.DefinitionId,
            DefinitionVersionId = instance.DefinitionVersionId,
            Version = instance.Version,
            Name = instance.Name,
        });
    }
}