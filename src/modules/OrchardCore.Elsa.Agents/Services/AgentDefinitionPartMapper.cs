using Elsa.Agents.Persistence.Entities;
using Elsa.Workflows;
using OrchardCore.Elsa.Agents.Parts;

namespace OrchardCore.Elsa.Agents.Services;

public class AgentDefinitionPartMapper(IApiSerializer apiSerializer)
{
    public AgentDefinition Map(AgentDefinitionPart part)
    {
        var agentConfig = part.SerializedData != null!
            ? apiSerializer.Deserialize<global::Elsa.Agents.AgentConfig>(part.SerializedData)
            : new();

        return new AgentDefinition
        {
            Id = part.DefinitionId,
            Name = part.Name,
            Description = part.Description,
            AgentConfig = agentConfig,
            TenantId = null! // OrchardCore handles multi-tenancy differently
        };
    }

    public void Map(AgentDefinition source, AgentDefinitionPart target)
    {
        target.DefinitionId = source.Id;
        target.Name = source.Name!;
        target.Description = source.Description;
        target.SerializedData = apiSerializer.Serialize(source.AgentConfig);
    }
}
