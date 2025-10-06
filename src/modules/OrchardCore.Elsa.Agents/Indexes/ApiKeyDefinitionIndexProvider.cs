using Elsa.Agents.Persistence.Entities;
using YesSql.Indexes;

namespace OrchardCore.Elsa.Agents.Indexes;

public class ApiKeyDefinitionIndexProvider : IndexProvider<ApiKeyDefinition>
{
    public ApiKeyDefinitionIndexProvider()
    {
        CollectionName = ElsaAgentCollections.AgentApiKeys;
    }

    public override void Describe(DescribeContext<ApiKeyDefinition> context)
    {
        context.For<ApiKeyDefinitionIndex>()
            .Map(entity => new ApiKeyDefinitionIndex
            {
                ApiKeyId = entity.Id!,
                Name = entity.Name
            });
    }
}
