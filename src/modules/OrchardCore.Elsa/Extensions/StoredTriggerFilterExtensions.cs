using Elsa.Workflows.Runtime.Entities;
using Elsa.Workflows.Runtime.Filters;
using OrchardCore.Elsa.Indexes;
using YesSql;
using YesSql.Services;

namespace OrchardCore.Elsa.Extensions;

public static class StoredTriggerFilterExtensions
{
    public static IQuery<StoredTrigger, StoredTriggerIndex> Apply(this TriggerFilter filter, IQuery<StoredTrigger, StoredTriggerIndex> query)
    {
        if (filter.Id != null) query = query.Where(x => x.TriggerId == filter.Id);
        if (filter.Ids != null) query = query.Where(x => x.TriggerId.IsIn(filter.Ids));
        if (filter.WorkflowDefinitionId != null) query = query.Where(x => x.WorkflowDefinitionId == filter.WorkflowDefinitionId);
        if (filter.WorkflowDefinitionIds != null) query = query.Where(x => x.WorkflowDefinitionId.IsIn(filter.WorkflowDefinitionIds));
        if (filter.WorkflowDefinitionVersionId != null) query = query.Where(x => x.WorkflowDefinitionVersionId == filter.WorkflowDefinitionVersionId);
        if (filter.WorkflowDefinitionVersionIds != null) query = query.Where(x => x.WorkflowDefinitionVersionId.IsIn(filter.WorkflowDefinitionVersionIds));
        if (filter.Name != null) query = query.Where(x => x.Name == filter.Name);
        if (filter.Names != null) query = query.Where(x => x.Name.IsIn(filter.Names));
        if (filter.Hash != null) query = query.Where(x => x.Hash == filter.Hash);
        
        return query;
    }
    
    public static IQueryIndex<StoredTriggerIndex> Apply(this TriggerFilter filter, IQueryIndex<StoredTriggerIndex> query)
    {
        if (filter.Id != null) query = query.Where(x => x.TriggerId == filter.Id);
        if (filter.Ids != null) query = query.Where(x => x.TriggerId.IsIn(filter.Ids));
        if (filter.WorkflowDefinitionId != null) query = query.Where(x => x.WorkflowDefinitionId == filter.WorkflowDefinitionId);
        if (filter.WorkflowDefinitionIds != null) query = query.Where(x => x.WorkflowDefinitionId.IsIn(filter.WorkflowDefinitionIds));
        if (filter.WorkflowDefinitionVersionId != null) query = query.Where(x => x.WorkflowDefinitionVersionId == filter.WorkflowDefinitionVersionId);
        if (filter.WorkflowDefinitionVersionIds != null) query = query.Where(x => x.WorkflowDefinitionVersionId.IsIn(filter.WorkflowDefinitionVersionIds));
        if (filter.Name != null) query = query.Where(x => x.Name == filter.Name);
        if (filter.Names != null) query = query.Where(x => x.Name.IsIn(filter.Names));
        if (filter.Hash != null) query = query.Where(x => x.Hash == filter.Hash);
        
        return query;
    }
}