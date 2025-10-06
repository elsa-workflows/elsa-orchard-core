using Elsa.Agents.Persistence.Contracts;
using Elsa.Agents.Persistence.Entities;
using Elsa.Agents.Persistence.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.Elsa;
using OrchardCore.Elsa.Agents.Extensions;
using OrchardCore.Elsa.Agents.Indexes;
using OrchardCore.Elsa.Agents.Parts;
using OrchardCore.Elsa.Agents.Services;
using OrchardCore.Modules;
using YesSql;

namespace OrchardCore.Elsa.Agents.Stores;

public class AgentStore(
    ISession session,
    IServiceProvider serviceProvider,
    ILogger<AgentStore> logger) : IAgentStore
{
    private const string Collection = ElsaCollections.AgentDefinitions;

    private readonly Lazy<IContentManager> _contentManager = new(serviceProvider.GetRequiredService<IContentManager>);
    private readonly Lazy<AgentDefinitionPartMapper> _mapper = new(serviceProvider.GetRequiredService<AgentDefinitionPartMapper>);
    private readonly Lazy<AgentDefinitionPartSerializer> _serializer = new(serviceProvider.GetRequiredService<AgentDefinitionPartSerializer>);
    private IEnumerable<IContentHandler>? _contentHandlers;
    private IEnumerable<IContentHandler>? _reversedHandlers;

    private IContentManager ContentManager => _contentManager.Value;
    private AgentDefinitionPartMapper Mapper => _mapper.Value;
    private AgentDefinitionPartSerializer Serializer => _serializer.Value;
    private IEnumerable<IContentHandler> ContentHandlers => _contentHandlers ??= serviceProvider.GetServices<IContentHandler>().ToArray();
    private IEnumerable<IContentHandler> ReversedHandlers => _reversedHandlers ??= ContentHandlers.Reverse().ToArray();

    public async Task AddAsync(AgentDefinition entity, CancellationToken cancellationToken = default)
    {
        await SaveAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(AgentDefinition entity, CancellationToken cancellationToken = default)
    {
        await SaveAsync(entity, cancellationToken);
    }

    public async Task<AgentDefinition?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = new AgentDefinitionFilter { Id = id };
        return await FindAsync(filter, cancellationToken);
    }

    public async Task<AgentDefinition?> FindAsync(AgentDefinitionFilter filter, CancellationToken cancellationToken = default)
    {
        var part = await Query(filter).FirstOrDefaultAsync(cancellationToken);
        return Map(part);
    }

    public async Task<IEnumerable<AgentDefinition>> ListAsync(CancellationToken cancellationToken = default)
    {
        var parts = await session.Query<ContentItem, AgentDefinitionIndex>(Collection).ListAsync(cancellationToken);
        return Map(parts);
    }

    public async Task DeleteAsync(AgentDefinition entity, CancellationToken cancellationToken = default)
    {
        var filter = new AgentDefinitionFilter { Id = entity.Id };
        await DeleteManyAsync(filter, cancellationToken);
    }

    public async Task<long> DeleteManyAsync(AgentDefinitionFilter filter, CancellationToken cancellationToken = default)
    {
        var query = await Query(filter).ListAsync(cancellationToken);
        var count = 0;

        foreach (var contentItem in query)
        {
            await RemoveContentItemAsync(contentItem, cancellationToken);
            count++;
        }

        await session.SaveChangesAsync(cancellationToken);
        return count;
    }

    private async Task SaveAsync(AgentDefinition definition, CancellationToken cancellationToken = default)
    {
        var contentItem = definition.Id != null! ? await GetContentItemByDefinitionIdAsync(definition.Id, cancellationToken) : null;

        if (contentItem == null)
            contentItem = await ContentManager.NewAsync("AgentDefinition");

        contentItem.DisplayText = definition.Name;
        await contentItem.AlterAsync<AgentDefinitionPart>(async part =>
        {
            Mapper.Map(definition, part);
            await Task.CompletedTask;
        });

        if (contentItem.ContentItemId == null!)
        {
            await ContentManager.CreateAsync(contentItem);
            definition.Id = contentItem.ContentItemId!;

            contentItem.Alter<AgentDefinitionPart>(part =>
            {
                part.DefinitionId = definition.Id;
                Serializer.UpdateSerializedData(part);
            });
        }

        await ContentManager.UpdateAsync(contentItem);

        definition.Id = contentItem.ContentItemId!;
    }

    private async Task<ContentItem?> GetContentItemByDefinitionIdAsync(string definitionId, CancellationToken cancellationToken)
    {
        return await session.Query<ContentItem, AgentDefinitionIndex>(Collection)
            .Where(x => x.DefinitionId == definitionId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private IQuery<ContentItem, AgentDefinitionIndex> Query(AgentDefinitionFilter filter)
    {
        var query = session.Query<ContentItem, AgentDefinitionIndex>(Collection).Apply(filter);
        return query;
    }

    private AgentDefinition? Map(ContentItem? contentItem)
    {
        return Map(contentItem?.As<AgentDefinitionPart>());
    }

    private AgentDefinition? Map(AgentDefinitionPart? part)
    {
        return part == null ? null : Mapper.Map(part);
    }

    private IEnumerable<AgentDefinition> Map(IEnumerable<ContentItem> contentItems)
    {
        return contentItems.Select(x => Mapper.Map(x.As<AgentDefinitionPart>()));
    }

    private async Task RemoveContentItemAsync(ContentItem contentItem, CancellationToken cancellationToken = default)
    {
        var removeContext = new RemoveContentContext(contentItem);
        await ContentHandlers.InvokeAsync((handler, context) => handler.RemovingAsync(context), removeContext, logger);
        session.Delete(contentItem, Collection);
        await ReversedHandlers.InvokeAsync((handler, context) => handler.RemovedAsync(context), removeContext, logger);
    }
}