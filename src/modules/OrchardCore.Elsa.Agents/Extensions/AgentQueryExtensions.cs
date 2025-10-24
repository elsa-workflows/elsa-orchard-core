using Elsa.Agents.Persistence.Filters;
using OrchardCore.Elsa.Agents.Indexes;
using YesSql;

namespace OrchardCore.Elsa.Agents.Extensions;

public static class AgentQueryExtensions
{
    public static IQueryIndex<AgentIndex> Apply(this IQueryIndex<AgentIndex> query, AgentDefinitionFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.Id))
            query = query.Where(x => x.AgentId == filter.Id);

        if (filter.Ids is { Count: > 0 })
            query = query.Where(x => filter.Ids!.Contains(x.AgentId));

        if (!string.IsNullOrWhiteSpace(filter.NotId))
            query = query.Where(x => x.AgentId != filter.NotId);

        if (!string.IsNullOrWhiteSpace(filter.Name))
            query = query.Where(x => x.Name == filter.Name);

        return query;
    }
}
