using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Common.Models;
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
    IServiceProvider serviceProvider) : IWorkflowDefinitionStore
{
    private const string Collection = ElsaCollections.WorkflowDefinitions;

    private readonly Lazy<IContentManager> _contentManager = new(serviceProvider.GetRequiredService<IContentManager>);
    private readonly Lazy<WorkflowDefinitionMapper> _workflowDefinitionMapper = new(serviceProvider.GetRequiredService<WorkflowDefinitionMapper>);
    private readonly Lazy<WorkflowDefinitionPartMapper> _workflowDefinitionPartMapper = new(serviceProvider.GetRequiredService<WorkflowDefinitionPartMapper>);
    private readonly Lazy<WorkflowDefinitionPartSerializer> _workflowDefinitionPartSerializer = new(serviceProvider.GetRequiredService<WorkflowDefinitionPartSerializer>);
    private IContentManager ContentManager => _contentManager.Value;
    private WorkflowDefinitionMapper WorkflowDefinitionMapper => _workflowDefinitionMapper.Value;
    private WorkflowDefinitionPartMapper WorkflowDefinitionPartMapper => _workflowDefinitionPartMapper.Value;
    private WorkflowDefinitionPartSerializer WorkflowDefinitionPartSerializer => _workflowDefinitionPartSerializer.Value;

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
        var contentItem = definition.Id != null! ? await ContentManager.GetVersionAsync(definition.Id) : null;

        if (contentItem == null) 
            contentItem = await ContentManager.NewAsync("WorkflowDefinition");
        
        contentItem.DisplayText = definition.Name;
        await contentItem.AlterAsync<WorkflowDefinitionPart>(async part =>
        {
            var model = await WorkflowDefinitionMapper.MapAsync(definition, cancellationToken);
            WorkflowDefinitionPartMapper.Map(model, part);
            part.DefinitionVersionId = contentItem.ContentItemVersionId;
            part.DefinitionId = contentItem.ContentItemId;
        });

        if (contentItem.ContentItemVersionId == null!)
        {
            await ContentManager.CreateAsync(contentItem, VersionOptions.Draft);
            definition.Id = contentItem.ContentItemVersionId!;

            contentItem.Alter<WorkflowDefinitionPart>(part =>
            {
                part.DefinitionVersionId = definition.Id;
                WorkflowDefinitionPartSerializer.UpdateSerializedData(part);
            });
        }

        await ContentManager.SaveDraftAsync(contentItem);
        
        definition.Id = contentItem.ContentItemVersionId!;
        definition.DefinitionId = contentItem.ContentItemId!;
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
        return await session.Query<WorkflowDefinitionPart, WorkflowDefinitionIndex>(Collection).CountAsync();
    }

    public async Task<bool> GetIsNameUnique(string name, string? definitionId = null, CancellationToken cancellationToken = default)
    {
        var query = session.Query<WorkflowDefinitionPart, WorkflowDefinitionIndex>(Collection).Where(x => x.Name == name);

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
        var query = session.Query<ContentItem, WorkflowDefinitionIndex>(Collection).Apply(filter);
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