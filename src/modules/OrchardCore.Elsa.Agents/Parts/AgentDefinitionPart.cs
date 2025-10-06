using OrchardCore.ContentManagement;

namespace OrchardCore.Elsa.Agents.Parts;

public class AgentDefinitionPart : ContentPart
{
    public string DefinitionId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string SerializedData { get; set; } = null!;
}
