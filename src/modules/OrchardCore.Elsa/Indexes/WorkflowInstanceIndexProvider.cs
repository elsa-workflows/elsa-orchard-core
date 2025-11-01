using JetBrains.Annotations;
using OrchardCore.Elsa.Documents;
using YesSql.Indexes;

namespace OrchardCore.Elsa.Indexes;

[UsedImplicitly]
public class WorkflowInstanceIndexProvider : IndexProvider<WorkflowInstanceDocument>
{
    public WorkflowInstanceIndexProvider()
    {
        CollectionName = ElsaCollections.WorkflowInstances;
    }

    public override void Describe(DescribeContext<WorkflowInstanceDocument> context)
    {
        context.For<WorkflowInstanceIndex>().Map(document => new()
        {
            InstanceId = document.InstanceId,
            DefinitionId = document.DefinitionId,
            DefinitionVersionId = document.DefinitionVersionId,
            Version = document.Version,
            Name = document.Name,
            Status = document.Status,
            CreatedAt = document.CreatedAt,
            FinishedAt = document.FinishedAt,
            CorrelationId = document.CorrelationId,
            IncidentCount = document.IncidentCount,
            IsSystem = document.IsSystem,
            SubStatus = document.SubStatus,
            UpdatedAt = document.UpdatedAt,
            ParentInstanceId = document.ParentWorkflowInstanceId,
            HasIncidents = document.IncidentCount > 0
        });
    }
}