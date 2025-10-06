using Elsa.Agents.Persistence.Filters;
using OrchardCore.Elsa.Agents.Indexes;
using YesSql;

namespace OrchardCore.Elsa.Agents.Extensions;

public static class ApiKeyQueryExtensions
{
    public static IQueryIndex<ApiKeyDefinitionIndex> Apply(this IQueryIndex<ApiKeyDefinitionIndex> query, ApiKeyDefinitionFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.Id))
            query = query.Where(x => x.ApiKeyId == filter.Id);

        if (filter.Ids is { Count: > 0 })
            query = query.Where(x => filter.Ids!.Contains(x.ApiKeyId));

        if (!string.IsNullOrWhiteSpace(filter.NotId))
            query = query.Where(x => x.ApiKeyId != filter.NotId);

        if (!string.IsNullOrWhiteSpace(filter.Name))
            query = query.Where(x => x.Name == filter.Name);

        return query;
    }
}
