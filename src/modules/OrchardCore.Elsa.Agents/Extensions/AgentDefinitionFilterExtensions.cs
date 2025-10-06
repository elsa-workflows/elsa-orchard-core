using Elsa.Agents.Persistence.Filters;
using OrchardCore.ContentManagement;
using OrchardCore.Elsa.Agents.Indexes;
using YesSql;
using YesSql.Services;

namespace OrchardCore.Elsa.Agents.Extensions;

public static class AgentDefinitionFilterExtensions
{
    public static IQuery<ContentItem, AgentDefinitionIndex> Apply(this IQuery<ContentItem, AgentDefinitionIndex> query, AgentDefinitionFilter filter)
    {
        if (filter.Id != null) query = query.Where(x => x.DefinitionId == filter.Id);
        if (filter.Ids != null) query = query.Where(x => x.DefinitionId.IsIn(filter.Ids));
        if (filter.Name != null) query = query.Where(x => x.Name == filter.Name);
        if (filter.NotId != null) query = query.Where(x => x.DefinitionId != filter.NotId);

        return query;
    }
}
