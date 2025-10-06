using Elsa.Agents;
using Elsa.Workflows;
using OrchardCore.Elsa.Agents.Parts;

namespace OrchardCore.Elsa.Agents.Services;

public class AgentDefinitionPartSerializer(IApiSerializer apiSerializer)
{
    public AgentConfig UpdateSerializedData(AgentDefinitionPart part)
    {
        var config = part.SerializedData != null!
            ? apiSerializer.Deserialize<AgentConfig>(part.SerializedData)
            : new();

        part.SerializedData = apiSerializer.Serialize(config);
        return config;
    }
}
