using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Common.Models;
using Elsa.Workflows;
using Elsa.Workflows.Management;
using Elsa.Workflows.Management.Entities;
using Elsa.Workflows.Management.Filters;
using Elsa.Workflows.Management.Mappers;
using Elsa.Workflows.Management.Models;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.ElsaWorkflows.Extensions;
using OrchardCore.ElsaWorkflows.Indexes;
using OrchardCore.ElsaWorkflows.Parts;
using OrchardCore.ElsaWorkflows.Services;
using YesSql;
using VersionOptions = OrchardCore.ContentManagement.VersionOptions;

namespace OrchardCore.ElsaWorkflows.Stores;

public class ElsaWorkflowDefinitionStore(
    ISession session,
    IApiSerializer apiSerializer,
    IServiceProvider serviceProvider) : IWorkflowDefinitionStore
{
    private readonly Lazy<IContentManager> _contentManager = new(serviceProvider.GetRequiredService<IContentManager>);
    private readonly Lazy<WorkflowDefinitionMapper> _workflowDefinitionMapper = new(serviceProvider.GetRequiredService<WorkflowDefinitionMapper>);
    private readonly Lazy<WorkflowDefinitionPartMapper> _workflowDefinitionPartMapper = new(serviceProvider.GetRequiredService<WorkflowDefinitionPartMapper>);
    private IContentManager ContentManager => _contentManager.Value;
    private WorkflowDefinitionMapper WorkflowDefinitionMapper => _workflowDefinitionMapper.Value;
    private WorkflowDefinitionPartMapper WorkflowDefinitionPartMapper => _workflowDefinitionPartMapper.Value;

    public async Task<WorkflowDefinition?> FindAsync(WorkflowDefinitionFilter filter, CancellationToken cancellationToken = default)
    {
        var part = await Query(filter).FirstOrDefaultAsync();
        return Map(part);
    }

    public async Task<WorkflowDefinition?> FindAsync<TOrderBy>(WorkflowDefinitionFilter filter, WorkflowDefinitionOrder<TOrderBy> order, CancellationToken cancellationToken = default)
    {
        var part = await Query(filter, order).FirstOrDefaultAsync();
        return Map(part);
    }

    public async Task<Page<WorkflowDefinition>> FindManyAsync(WorkflowDefinitionFilter filter, PageArgs pageArgs, CancellationToken cancellationToken = default)
    {
        var query = Query(filter, pageArgs);
        var count = await query.CountAsync();
        var parts = await query.ListAsync();
        var definitions = Map(parts).ToList();

        return Page.Of(definitions, count);
    }

    public async Task<Page<WorkflowDefinition>> FindManyAsync<TOrderBy>(WorkflowDefinitionFilter filter, WorkflowDefinitionOrder<TOrderBy> order, PageArgs pageArgs, CancellationToken cancellationToken = default)
    {
        var query = Query(filter, order, pageArgs);
        var count = await query.CountAsync();
        var parts = await query.ListAsync();
        var definitions = Map(parts).ToList();

        return Page.Of(definitions, count);
    }

    public async Task<IEnumerable<WorkflowDefinition>> FindManyAsync(WorkflowDefinitionFilter filter, CancellationToken cancellationToken = default)
    {
        var query = Query(filter);
        var parts = await query.ListAsync();
        return Map(parts);
    }

    public async Task<IEnumerable<WorkflowDefinition>> FindManyAsync<TOrderBy>(WorkflowDefinitionFilter filter, WorkflowDefinitionOrder<TOrderBy> order, CancellationToken cancellationToken = default)
    {
        var query = Query(filter, order);
        var parts = await query.ListAsync();
        return Map(parts);
    }

    public async Task<Page<WorkflowDefinitionSummary>> FindSummariesAsync(WorkflowDefinitionFilter filter, PageArgs pageArgs, CancellationToken cancellationToken = default)
    {
        var query = Query(filter, pageArgs);
        var count = await query.CountAsync();
        var parts = await query.ListAsync();
        var definitions = MapSummaries(parts).ToList();

        return Page.Of(definitions, count);
    }

    public async Task<Page<WorkflowDefinitionSummary>> FindSummariesAsync<TOrderBy>(WorkflowDefinitionFilter filter, WorkflowDefinitionOrder<TOrderBy> order, PageArgs pageArgs, CancellationToken cancellationToken = default)
    {
        var query = Query(filter, order, pageArgs);
        var count = await query.CountAsync();
        var parts = await query.ListAsync();
        var definitions = MapSummaries(parts).ToList();

        return Page.Of(definitions, count);
    }

    public async Task<IEnumerable<WorkflowDefinitionSummary>> FindSummariesAsync(WorkflowDefinitionFilter filter, CancellationToken cancellationToken = default)
    {
        var query = Query(filter);
        var parts = await query.ListAsync();
        return MapSummaries(parts);
    }

    public async Task<IEnumerable<WorkflowDefinitionSummary>> FindSummariesAsync<TOrderBy>(WorkflowDefinitionFilter filter, WorkflowDefinitionOrder<TOrderBy> order, CancellationToken cancellationToken = default)
    {
        var query = Query(filter, order);
        var parts = await query.ListAsync();
        return MapSummaries(parts);
    }

    public async Task<WorkflowDefinition?> FindLastVersionAsync(WorkflowDefinitionFilter filter, CancellationToken cancellationToken)
    {
        var part = await Query(filter).OrderByDescending(x => x.Version).FirstOrDefaultAsync();
        return Map(part);
    }

    public async Task SaveAsync(WorkflowDefinition definition, CancellationToken cancellationToken = default)
    {
        var contentItem = await ContentManager.GetVersionAsync(definition.Id);

        if (contentItem == null)
        {
            contentItem = await ContentManager.NewAsync("WorkflowDefinition");
            await ContentManager.CreateAsync(contentItem, VersionOptions.Draft);
            definition.Id = contentItem.ContentItemVersionId;
        }
        
        var workflowDefinitionModel = await WorkflowDefinitionMapper.MapAsync(definition, cancellationToken);
        contentItem.DisplayText = definition.Name;
        contentItem.Alter<WorkflowDefinitionPart>(part =>
        {
            part.Name = definition.Name;
            part.Description = definition.Description;
            part.Version = definition.Version;
            part.DefinitionId = definition.DefinitionId;
            part.DefinitionVersionId = definition.Id;
            part.IsPublished = definition.IsPublished;
            part.IsLatest = definition.IsLatest;
            part.IsReadonly = definition.IsReadonly;
            part.IsSystem = definition.IsSystem;
            part.MaterializerName = definition.MaterializerName;
            part.ProviderName = definition.ProviderName;
            part.ToolVersion = definition.ToolVersion;
            part.UsableAsActivity = definition.Options.UsableAsActivity == true;
            part.SerializedData = apiSerializer.Serialize(workflowDefinitionModel);
        });

        await ContentManager.SaveDraftAsync(contentItem);
    }

    public async Task SaveManyAsync(IEnumerable<WorkflowDefinition> definitions, CancellationToken cancellationToken = default)
    {
        foreach (var definition in definitions)
            await SaveAsync(definition, cancellationToken);
    }

    public async Task<long> DeleteAsync(WorkflowDefinitionFilter filter, CancellationToken cancellationToken = default)
    {
        var query = await Query(filter).ListAsync();
        var count = 0;

        foreach (var part in query)
        {
            await ContentManager.RemoveAsync(part.ContentItem);
            count++;
        }

        return count;
    }

    public async Task<bool> AnyAsync(WorkflowDefinitionFilter filter, CancellationToken cancellationToken = default)
    {
        return await Query(filter).CountAsync() > 0;
    }

    public async Task<long> CountDistinctAsync(CancellationToken cancellationToken = default)
    {
        return await session.Query<WorkflowDefinitionPart, WorkflowDefinitionIndex>().CountAsync();
    }

    public async Task<bool> GetIsNameUnique(string name, string? definitionId = null, CancellationToken cancellationToken = default)
    {
        var query = session.Query<WorkflowDefinitionPart, WorkflowDefinitionIndex>().Where(x => x.Name == name);

        if (definitionId != null)
            query = query.Where(x => x.DefinitionId != definitionId);

        return await query.CountAsync() == 0;
    }

    private IQuery<ContentItem, WorkflowDefinitionIndex> Query(WorkflowDefinitionFilter filter, PageArgs? pageArgs = null)
    {
        return Query<string>(filter, null, pageArgs);
    }

    private IQuery<ContentItem, WorkflowDefinitionIndex> Query<TOrderBy>(WorkflowDefinitionFilter filter, WorkflowDefinitionOrder<TOrderBy>? order = null, PageArgs? pageArgs = null)
    {
        var query = session.Query<ContentItem, WorkflowDefinitionIndex>().Apply(filter);
        if (order != null) query = query.Apply(order);

        if (pageArgs != null)
        {
            if (pageArgs.Offset != null) query.Skip(pageArgs.Offset.Value);
            if (pageArgs.Limit != null) query.Take(pageArgs.Limit.Value);
        }

        return query;
    }

    private WorkflowDefinition? Map(ContentItem? contentItem)
    {
        return Map(contentItem?.As<WorkflowDefinitionPart>());
    }
    
    private WorkflowDefinition? Map(WorkflowDefinitionPart? part)
    {
        return part == null ? null : WorkflowDefinitionPartMapper.Map(part);
    }

    private WorkflowDefinitionSummary? MapSummary(WorkflowDefinitionPart? part)
    {
        return part == null ? null : WorkflowDefinitionPartMapper.MapSummary(part);
    }

    private IEnumerable<WorkflowDefinition> Map(IEnumerable<ContentItem> contentItems)
    {
        return contentItems.Select(x => WorkflowDefinitionPartMapper.Map(x.As<WorkflowDefinitionPart>()));
    }
    
    private IEnumerable<WorkflowDefinition> Map(IEnumerable<WorkflowDefinitionPart> parts)
    {
        return parts.Select(WorkflowDefinitionPartMapper.Map);
    }

    private IEnumerable<WorkflowDefinitionSummary> MapSummaries(IEnumerable<ContentItem> contentItems)
    {
        return contentItems.Select(x => WorkflowDefinitionPartMapper.MapSummary(x.As<WorkflowDefinitionPart>()));
    }

    private IEnumerable<WorkflowDefinitionSummary> MapSummaries(IEnumerable<WorkflowDefinitionPart> parts)
    {
        return parts.Select(WorkflowDefinitionPartMapper.MapSummary);
    }
}