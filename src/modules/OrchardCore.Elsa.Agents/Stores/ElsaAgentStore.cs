using System.Linq;
using Elsa.Agents.Persistence.Contracts;
using Elsa.Agents.Persistence.Entities;
using Elsa.Agents.Persistence.Filters;
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
    IContentManager contentManager,
    ShellSettings shellSettings,
    ILogger<ElsaAgentStore> logger) : IAgentStore
{
    public async Task AddAsync(AgentDefinition entity, CancellationToken cancellationToken = default)
    {
        var contentItem = await contentManager.NewAsync(AgentConstants.AgentContentType);
        await ApplyDefinitionAsync(contentItem, entity, cancellationToken);
        await contentManager.CreateAsync(contentItem, VersionOptions.Draft);
        await contentManager.PublishAsync(contentItem);

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
        await ApplyDefinitionAsync(contentItem, entity, cancellationToken);
        await contentManager.SaveDraftAsync(contentItem);

        if (wasPublished)
            await contentManager.PublishAsync(contentItem);
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
        var contentItem = await contentManager.GetAsync(index.ContentItemId, version);
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
            var contentItem = await contentManager.GetAsync(index.ContentItemId, VersionOptions.Published);
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

        await contentManager.RemoveAsync(contentItem);
    }

    public async Task<long> DeleteManyAsync(AgentDefinitionFilter filter, CancellationToken cancellationToken = default)
    {
        var indexes = await session.QueryIndex<AgentIndex>().Apply(filter).ListAsync(cancellationToken);
        long count = 0;

        foreach (var index in indexes)
        {
            var contentItem = await contentManager.GetAsync(index.ContentItemId, VersionOptions.Latest);
            if (contentItem == null)
                continue;

            await contentManager.RemoveAsync(contentItem);
            count++;
        }

        return count;
    }

    private async Task ApplyDefinitionAsync(ContentItem contentItem, AgentDefinition definition, CancellationToken cancellationToken)
    {
        var agentId = string.IsNullOrWhiteSpace(definition.Id) ? contentItem.ContentItemId : definition.Id;
        definition.Id = agentId;
        definition.TenantId = shellSettings.Name;

        contentItem.DisplayText = definition.Name;

        await contentItem.AlterAsync<AgentPart>(part =>
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

        return await contentManager.GetAsync(index.ContentItemId, version);
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
