using YesSql.Indexes;

namespace OrchardCore.Elsa.Agents.Indexes;

public class AgentDefinitionIndex : MapIndex
{
    public string DefinitionId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
