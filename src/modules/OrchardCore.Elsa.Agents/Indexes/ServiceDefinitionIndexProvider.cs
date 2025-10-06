using Elsa.Agents.Persistence.Entities;
using YesSql.Indexes;

namespace OrchardCore.Elsa.Agents.Indexes;

public class ServiceDefinitionIndexProvider : IndexProvider<ServiceDefinition>
{
    public ServiceDefinitionIndexProvider()
    {
        CollectionName = ElsaAgentCollections.AgentServices;
    }

    public override void Describe(DescribeContext<ServiceDefinition> context)
    {
        context.For<ServiceDefinitionIndex>()
            .Map(entity => new ServiceDefinitionIndex
            {
                ServiceId = entity.Id!,
                Name = entity.Name,
                Type = entity.Type
            });
    }
}
