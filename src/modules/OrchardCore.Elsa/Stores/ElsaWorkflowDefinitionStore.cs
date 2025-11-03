using Elsa.Common.Models;
using Elsa.Workflows.Management;
using Elsa.Workflows.Management.Entities;
using Elsa.Workflows.Management.Filters;
using Elsa.Workflows.Management.Mappers;
using Elsa.Workflows.Management.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.ContentManagement.Records;
using OrchardCore.Elsa.Indexes;
using OrchardCore.Elsa.Parts;
using OrchardCore.Elsa.Services;
using OrchardCore.Elsa.Extensions;
using OrchardCore.Modules;
using YesSql;
using VersionOptions = OrchardCore.ContentManagement.VersionOptions;

namespace OrchardCore.Elsa.Stores;

public class ElsaWorkflowDefinitionStore(
    ISession session,
    IServiceProvider serviceProvider,
    ILogger<ElsaWorkflowDefinitionStore> logger) : IWorkflowDefinitionStore
{
    private const string Collection = ElsaCollections.WorkflowDefinitions;

    private readonly Lazy<IContentManager> _contentManager = new(serviceProvider.GetRequiredService<IContentManager>);
    private readonly Lazy<WorkflowDefinitionMapper> _workflowDefinitionMapper = new(serviceProvider.GetRequiredService<WorkflowDefinitionMapper>);
    private readonly Lazy<WorkflowDefinitionPartMapper> _workflowDefinitionPartMapper = new(serviceProvider.GetRequiredService<WorkflowDefinitionPartMapper>);
    private readonly Lazy<WorkflowDefinitionPartSerializer> _workflowDefinitionPartSerializer = new(serviceProvider.GetRequiredService<WorkflowDefinitionPartSerializer>);
    private IEnumerable<IContentHandler>? _contentHandlers;
    private IEnumerable<IContentHandler>? _reversedHandlers;
    private IContentManager ContentManager => _contentManager.Value;
    private WorkflowDefinitionMapper WorkflowDefinitionMapper => _workflowDefinitionMapper.Value;
    private WorkflowDefinitionPartMapper WorkflowDefinitionPartMapper => _workflowDefinitionPartMapper.Value;
    private WorkflowDefinitionPartSerializer WorkflowDefinitionPartSerializer => _workflowDefinitionPartSerializer.Value;
    private IEnumerable<IContentHandler> ContentHandlers => _contentHandlers ??= serviceProvider.GetServices<IContentHandler>().ToArray();
    private IEnumerable<IContentHandler> ReversedHandlers => _reversedHandlers ??= ContentHandlers.Reverse().ToArray();

    public async Task<WorkflowDefinition?> FindAsync(WorkflowDefinitionFilter filter, CancellationToken cancellationToken = default)
    {
        var part = await Query(filter).FirstOrDefaultAsync(cancellationToken);
        return Map(part);
    }

    public async Task<WorkflowDefinition?> FindAsync<TOrderBy>(WorkflowDefinitionFilter filter, WorkflowDefinitionOrder<TOrderBy> order, CancellationToken cancellationToken = default)
    {
        var part = await Query(filter, order).FirstOrDefaultAsync(cancellationToken);
        return Map(part);
    }

    public async Task<Page<WorkflowDefinition>> FindManyAsync(WorkflowDefinitionFilter filter, PageArgs pageArgs, CancellationToken cancellationToken = default)
    {
        var query = Query(filter, pageArgs);
        var count = await query.CountAsync(cancellationToken);
        var parts = await query.ListAsync(cancellationToken);
        var definitions = Map(parts).ToList();

        return Page.Of(definitions, count);
    }

    public async Task<Page<WorkflowDefinition>> FindManyAsync<TOrderBy>(WorkflowDefinitionFilter filter, WorkflowDefinitionOrder<TOrderBy> order, PageArgs pageArgs, CancellationToken cancellationToken = default)
    {
        var query = Query(filter, order, pageArgs);
        var count = await query.CountAsync(cancellationToken);
        var parts = await query.ListAsync(cancellationToken);
        var definitions = Map(parts).ToList();

        return Page.Of(definitions, count);
    }

    public async Task<IEnumerable<WorkflowDefinition>> FindManyAsync(WorkflowDefinitionFilter filter, CancellationToken cancellationToken = default)
    {
        var query = Query(filter);
        var parts = await query.ListAsync(cancellationToken);
        return Map(parts);
    }

    public async Task<IEnumerable<WorkflowDefinition>> FindManyAsync<TOrderBy>(WorkflowDefinitionFilter filter, WorkflowDefinitionOrder<TOrderBy> order, CancellationToken cancellationToken = default)
    {
        var query = Query(filter, order);
        var parts = await query.ListAsync(cancellationToken);
        return Map(parts);
    }

    public async Task<Page<WorkflowDefinitionSummary>> FindSummariesAsync(WorkflowDefinitionFilter filter, PageArgs pageArgs, CancellationToken cancellationToken = default)
    {
        var query = Query(filter, pageArgs);
        var count = await query.CountAsync(cancellationToken);
        var parts = await query.ListAsync(cancellationToken);
        var definitions = MapSummaries(parts).ToList();

        return Page.Of(definitions, count);
    }

    public async Task<Page<WorkflowDefinitionSummary>> FindSummariesAsync<TOrderBy>(WorkflowDefinitionFilter filter, WorkflowDefinitionOrder<TOrderBy> order, PageArgs pageArgs, CancellationToken cancellationToken = default)
    {
        var query = Query(filter, order, pageArgs);
        var count = await query.CountAsync(cancellationToken);
        var parts = await query.ListAsync(cancellationToken);
        var definitions = MapSummaries(parts).ToList();

        return Page.Of(definitions, count);
    }

    public async Task<IEnumerable<WorkflowDefinitionSummary>> FindSummariesAsync(WorkflowDefinitionFilter filter, CancellationToken cancellationToken = default)
    {
        var query = Query(filter);
        var parts = await query.ListAsync(cancellationToken);
        return MapSummaries(parts);
    }

    public async Task<IEnumerable<WorkflowDefinitionSummary>> FindSummariesAsync<TOrderBy>(WorkflowDefinitionFilter filter, WorkflowDefinitionOrder<TOrderBy> order, CancellationToken cancellationToken = default)
    {
        var query = Query(filter, order);
        var parts = await query.ListAsync(cancellationToken);
        return MapSummaries(parts);
    }

    public async Task<WorkflowDefinition?> FindLastVersionAsync(WorkflowDefinitionFilter filter, CancellationToken cancellationToken)
    {
        var part = await Query(filter).OrderByDescending(x => x.Version).FirstOrDefaultAsync(cancellationToken);
        return Map(part);
    }

    public async Task SaveAsync(WorkflowDefinition definition, CancellationToken cancellationToken = default)
    {
        var contentItem = string.IsNullOrEmpty(definition.Id) ? null : await ContentManager.GetVersionAsync(definition.Id);

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
        var query = await Query(filter).ListAsync(cancellationToken);
        var count = 0;

        foreach (var contentItem in query)
        {
            await RemoveContentItemVersionAsync(contentItem, cancellationToken);
            count++;
        }

        await session.FlushAsync(cancellationToken);
        return count;
    }

    public async Task<bool> AnyAsync(WorkflowDefinitionFilter filter, CancellationToken cancellationToken = default)
    {
        return await Query(filter).CountAsync(cancellationToken) > 0;
    }

    public async Task<long> CountDistinctAsync(CancellationToken cancellationToken = default)
    {
        return await session.Query<WorkflowDefinitionPart, WorkflowDefinitionIndex>(Collection).CountAsync(cancellationToken);
    }

    public async Task<bool> GetIsNameUnique(string name, string? definitionId = null, CancellationToken cancellationToken = default)
    {
        var query = session.Query<WorkflowDefinitionPart, WorkflowDefinitionIndex>(Collection).Where(x => x.Name == name);

        if (definitionId != null)
            query = query.Where(x => x.DefinitionId != definitionId);

        return await query.CountAsync(cancellationToken) == 0;
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

    private IEnumerable<WorkflowDefinition> Map(IEnumerable<ContentItem> contentItems)
    {
        return contentItems.Select(x => WorkflowDefinitionPartMapper.Map(x.As<WorkflowDefinitionPart>()));
    }

    private IEnumerable<WorkflowDefinitionSummary> MapSummaries(IEnumerable<ContentItem> contentItems)
    {
        return contentItems.Select(x => WorkflowDefinitionPartMapper.MapSummary(x.As<WorkflowDefinitionPart>()));
    }

    private async Task RemoveContentItemVersionAsync(ContentItem contentItem, CancellationToken cancellationToken = default)
    {
        var removeContext = new RemoveContentContext(contentItem);
        await ContentHandlers.InvokeAsync((handler, context) => handler.RemovingAsync(context), removeContext, logger);
        session.Delete(contentItem, Collection);
        await ReversedHandlers.InvokeAsync((handler, context) => handler.RemovedAsync(context), removeContext, logger);
    }
}