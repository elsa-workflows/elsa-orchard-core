using Elsa.Agents;
using OrchardCore.ContentManagement;

namespace OrchardCore.Elsa.Agents.Parts;

public class AgentPart : ContentPart
{
    public string AgentId { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public AgentConfig AgentConfig { get; set; } = new();
}
