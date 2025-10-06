using Elsa.Agents.Persistence.Filters;
using OrchardCore.Elsa.Agents.Indexes;
using YesSql;

namespace OrchardCore.Elsa.Agents.Extensions;

public static class ServiceQueryExtensions
{
    public static IQueryIndex<ServiceDefinitionIndex> Apply(this IQueryIndex<ServiceDefinitionIndex> query, ServiceDefinitionFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.Id))
            query = query.Where(x => x.ServiceId == filter.Id);

        if (filter.Ids is { Count: > 0 })
            query = query.Where(x => filter.Ids!.Contains(x.ServiceId));

        if (!string.IsNullOrWhiteSpace(filter.NotId))
            query = query.Where(x => x.ServiceId != filter.NotId);

        if (!string.IsNullOrWhiteSpace(filter.Name))
            query = query.Where(x => x.Name == filter.Name);

        return query;
    }
}
