using YesSql.Indexes;

namespace OrchardCore.ElsaWorkflows.Indexes;

public class WorkflowDefinitionIndex : MapIndex
{
    public string WorkflowDefinitionId { get; set; } = null!;
    public string Name { get; set; } = null!;
}