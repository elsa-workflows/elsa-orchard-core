using YesSql.Indexes;

namespace OrchardCore.Elsa.Indexes;

public class WorkflowDefinitionIndex : MapIndex
{
    public string DefinitionId { get; set; } = null!;
    public string DefinitionVersionId { get; set; } = null!;
    public int Version { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? MaterializerName { get; set; }
    public bool IsPublished { get; set; }
    public bool IsLatest { get; set; }
    public bool? UsableAsActivity { get; set; }
    public bool? IsSystem { get; set; }
    public bool? IsReadonly { get; set; }
}