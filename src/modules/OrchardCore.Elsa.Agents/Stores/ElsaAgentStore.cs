using Elsa.Agents.Persistence.Contracts;
using Elsa.Agents.Persistence.Entities;
using Elsa.Agents.Persistence.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrchardCore.ContentManagement;
using OrchardCore.Elsa.Agents.Extensions;
using OrchardCore.Elsa.Agents.Indexes;
using OrchardCore.Elsa.Agents.Parts;
using OrchardCore.Environment.Shell;
using YesSql;
using VersionOptions = OrchardCore.ContentManagement.VersionOptions;

namespace OrchardCore.Elsa.Agents.Stores;

public class ElsaAgentStore(
    ISession session,
    ShellSettings shellSettings,
    IServiceProvider serviceProvider,
    ILogger<ElsaAgentStore> logger) : IAgentStore
{
    private readonly Lazy<IContentManager> _contentManager = new(serviceProvider.GetRequiredService<IContentManager>);
    private IContentManager ContentManager => _contentManager.Value;
    
    public async Task AddAsync(AgentDefinition entity, CancellationToken cancellationToken = default)
    {
        var contentItem = await ContentManager.NewAsync(AgentConstants.AgentContentType);
        ApplyDefinition(contentItem, entity);
        await ContentManager.CreateAsync(contentItem, VersionOptions.Draft);
        await ContentManager.PublishAsync(contentItem);

        var part = contentItem.As<AgentPart>()!;
        entity.Id = part.AgentId;
        entity.TenantId = shellSettings.Name;
    }

    public async Task UpdateAsync(AgentDefinition entity, CancellationToken cancellationToken = default)
    {
        var contentItem = await LoadContentItemAsync(entity.Id, VersionOptions.Latest, cancellationToken);

        if (contentItem == null)
        {
            logger.LogWarning("Attempted to update agent {AgentId} but no content item was found.", entity.Id);
            return;
        }

        var wasPublished = contentItem.Published;
        ApplyDefinition(contentItem, entity);
        await ContentManager.SaveDraftAsync(contentItem);

        if (wasPublished)
            await ContentManager.PublishAsync(contentItem);
    }

    public async Task<AgentDefinition?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var contentItem = await LoadContentItemAsync(id, VersionOptions.Latest, cancellationToken)
                          ?? await LoadContentItemAsync(id, VersionOptions.Published, cancellationToken);

        return contentItem == null ? null : Map(contentItem.As<AgentPart>());
    }

    public async Task<AgentDefinition?> FindAsync(AgentDefinitionFilter filter, CancellationToken cancellationToken = default)
    {
        var index = await session.QueryIndex<AgentIndex>().Apply(filter).FirstOrDefaultAsync(cancellationToken);
        if (index == null)
            return null;

        var version = index.Latest ? VersionOptions.Latest : VersionOptions.Published;
        var contentItem = await ContentManager.GetAsync(index.ContentItemId, version);
        return contentItem == null ? null : Map(contentItem.As<AgentPart>());
    }

    public async Task<IEnumerable<AgentDefinition>> ListAsync(CancellationToken cancellationToken = default)
    {
        var indexes = await session
            .QueryIndex<AgentIndex>()
            .Where(x => x.Published)
            .ListAsync(cancellationToken);

        var results = new List<AgentDefinition>();

        foreach (var index in indexes)
        {
            var contentItem = await ContentManager.GetAsync(index.ContentItemId, VersionOptions.Published);
            if (contentItem == null)
                continue;

            results.Add(Map(contentItem.As<AgentPart>()));
        }

        return results;
    }

    public async Task DeleteAsync(AgentDefinition entity, CancellationToken cancellationToken = default)
    {
        var contentItem = await LoadContentItemAsync(entity.Id, VersionOptions.Latest, cancellationToken);
        if (contentItem == null)
            return;

        await ContentManager.RemoveAsync(contentItem);
    }

    public async Task<long> DeleteManyAsync(AgentDefinitionFilter filter, CancellationToken cancellationToken = default)
    {
        var indexes = await session.QueryIndex<AgentIndex>().Apply(filter).ListAsync(cancellationToken);
        long count = 0;

        foreach (var index in indexes)
        {
            var contentItem = await ContentManager.GetAsync(index.ContentItemId, VersionOptions.Latest);
            if (contentItem == null)
                continue;

            await ContentManager.RemoveAsync(contentItem);
            count++;
        }

        return count;
    }

    private void ApplyDefinition(ContentItem contentItem, AgentDefinition definition)
    {
        var agentId = string.IsNullOrWhiteSpace(definition.Id) ? contentItem.ContentItemId : definition.Id;
        definition.Id = agentId;
        definition.TenantId = shellSettings.Name;

        contentItem.DisplayText = definition.Name;

        contentItem.Alter<AgentPart>(part =>
        {
            part.AgentId = agentId;
            part.Name = definition.Name;
            part.Description = definition.Description;
            part.AgentConfig = definition.AgentConfig ?? new();
        });
    }

    private async Task<ContentItem?> LoadContentItemAsync(string id, VersionOptions version, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        var index = await session
            .QueryIndex<AgentIndex>()
            .Where(x => x.AgentId == id && (version == VersionOptions.Published ? x.Published : x.Latest))
            .FirstOrDefaultAsync(cancellationToken);

        if (index == null)
            return null;

        return await ContentManager.GetAsync(index.ContentItemId, version);
    }

    private AgentDefinition Map(AgentPart part)
    {
        return new AgentDefinition
        {
            Id = part.AgentId,
            Name = part.Name,
            Description = part.Description,
            AgentConfig = part.AgentConfig,
            TenantId = shellSettings.Name
        };
    }
}
