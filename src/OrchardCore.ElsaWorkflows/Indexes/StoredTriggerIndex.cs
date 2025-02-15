using YesSql.Indexes;

namespace OrchardCore.ElsaWorkflows.Indexes;

public class StoredTriggerIndex : MapIndex
{
    public string TriggerId { get; set; } = null!;
    public string WorkflowDefinitionId { get; set; } = null!;
    public string WorkflowDefinitionVersionId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string ActivityId { get; set; } = null!;
    public string? Hash { get; set; }
}