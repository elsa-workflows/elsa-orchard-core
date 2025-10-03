using Elsa.Workflows.Management.Filters;
using OrchardCore.ContentManagement;
using OrchardCore.Elsa.Indexes;
using YesSql;
using YesSql.Services;

namespace OrchardCore.Elsa.Extensions;

public static class WorkflowDefinitionFilterExtensions
{
    public static IQuery<ContentItem, WorkflowDefinitionIndex> Apply(this WorkflowDefinitionFilter filter, IQuery<ContentItem, WorkflowDefinitionIndex> query)
    {
        var definitionId = filter.DefinitionId ?? filter.DefinitionHandle?.DefinitionId;
        var versionOptions = filter.VersionOptions ?? filter.DefinitionHandle?.VersionOptions;
        var id = filter.Id ?? filter.DefinitionHandle?.DefinitionVersionId;
        var searchTerm = filter.SearchTerm;

        if (definitionId != null) query = query.Where(x => x.DefinitionId == definitionId);
        if (filter.DefinitionIds != null) query = query.Where(x => x.DefinitionId.IsIn(filter.DefinitionIds));
        if (id != null) query = query.Where(x => x.DefinitionVersionId == id);
        if (filter.Ids != null) query = query.Where(x => x.DefinitionVersionId.IsIn(filter.Ids));
        if (versionOptions != null) query = query.WithVersion(versionOptions.Value);
        if (filter.MaterializerName != null) query = query.Where(x => x.MaterializerName == filter.MaterializerName);
        if (filter.Name != null) query = query.Where(x => x.Name == filter.Name);
        if (filter.Names != null) query = query.Where(x => x.Name!.IsIn(filter.Names));
        if (filter.UsableAsActivity != null) query = query.Where(x => x.UsableAsActivity == filter.UsableAsActivity);
        if (!string.IsNullOrWhiteSpace(searchTerm)) query = query.Where(x => x.Name!.ToLower().Contains(searchTerm.ToLower()) || x.Description!.ToLower().Contains(searchTerm.ToLower()) || x.DefinitionVersionId.Contains(searchTerm) || x.DefinitionId.Contains(searchTerm));
        if (filter.IsSystem != null) query = query.Where(x => x.IsSystem == filter.IsSystem);
        if (filter.IsReadonly != null) query = query.Where(x => x.IsReadonly == filter.IsReadonly);

        return query;
    }
}