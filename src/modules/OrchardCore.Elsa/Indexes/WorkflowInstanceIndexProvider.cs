using Elsa.Workflows.Management.Entities;
using JetBrains.Annotations;
using YesSql.Indexes;

namespace OrchardCore.Elsa.Indexes;

[UsedImplicitly]
public class WorkflowInstanceIndexProvider : IndexProvider<WorkflowInstance>
{
    public WorkflowInstanceIndexProvider()
    {
        CollectionName = ElsaCollections.WorkflowInstances;
    }

    public override void Describe(DescribeContext<WorkflowInstance> context)
    {
        context.For<WorkflowInstanceIndex>().Map(instance => new()
        {
            InstanceId = instance.Id,
            DefinitionId = instance.DefinitionId,
            DefinitionVersionId = instance.DefinitionVersionId,
            Version = instance.Version,
            Name = instance.Name,
            Status = instance.Status,
            CreatedAt = instance.CreatedAt,
            FinishedAt = instance.FinishedAt,
            CorrelationId = instance.CorrelationId,
            IncidentCount = instance.IncidentCount,
            IsSystem = instance.IsSystem,
            SubStatus = instance.SubStatus,
            UpdatedAt = instance.UpdatedAt,
            ParentInstanceId = instance.ParentWorkflowInstanceId,
            HasIncidents = instance.IncidentCount > 0
        });
    }
}