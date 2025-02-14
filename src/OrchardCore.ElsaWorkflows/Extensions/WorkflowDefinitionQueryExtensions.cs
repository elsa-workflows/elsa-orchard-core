using Elsa.Common.Entities;
using Elsa.Workflows.Management.Entities;
using Elsa.Workflows.Management.Filters;
using OrchardCore.ContentManagement;
using OrchardCore.ElsaWorkflows.Indexes;
using YesSql;
using VersionOptions = Elsa.Common.Models.VersionOptions;

namespace OrchardCore.ElsaWorkflows.Extensions;

public static class WorkflowDefinitionQueryExtensions
{
    public static IQuery<ContentItem, WorkflowDefinitionIndex> WithVersion(this IQuery<ContentItem, WorkflowDefinitionIndex> query, VersionOptions versionOptions)
    {
        if (versionOptions.IsDraft)
            return query.Where(x => !x.IsPublished);
        if (versionOptions.IsLatest)
            return query.Where(x => x.IsLatest);
        if (versionOptions.IsPublished)
            return query.Where(x => x.IsPublished);
        if (versionOptions.IsLatestOrPublished)
            return query.Where(x => x.IsPublished || x.IsLatest);
        if (versionOptions.IsLatestAndPublished)
            return query.Where(x => x.IsPublished && x.IsLatest);
        if (versionOptions.Version > 0)
            return query.Where(x => x.Version == versionOptions.Version);

        return query;
    }
    
    public static IQuery<ContentItem, WorkflowDefinitionIndex> Apply(this IQuery<ContentItem, WorkflowDefinitionIndex> query, WorkflowDefinitionFilter filter)
    {
        return filter.Apply(query);
    }
    
    public static IQuery<ContentItem, WorkflowDefinitionIndex> Apply<TOrderBy>(this IQuery<ContentItem, WorkflowDefinitionIndex> query, WorkflowDefinitionOrder<TOrderBy> order)
    {
        var keySelector = ExpressionConverter.Convert<WorkflowDefinition, WorkflowDefinitionIndex, TOrderBy>(order.KeySelector);
        return order.Direction == OrderDirection.Ascending 
            ? query.OrderBy(keySelector) 
            : query.OrderByDescending(keySelector);
    }
}