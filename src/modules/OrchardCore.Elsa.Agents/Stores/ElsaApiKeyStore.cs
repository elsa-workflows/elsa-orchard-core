using Elsa.Agents.Persistence.Contracts;
using Elsa.Agents.Persistence.Entities;
using Elsa.Agents.Persistence.Filters;
using Microsoft.Extensions.Logging;
using OrchardCore.Elsa.Agents.Extensions;
using OrchardCore.Elsa.Agents.Indexes;
using OrchardCore.Environment.Shell;
using YesSql;

namespace OrchardCore.Elsa.Agents.Stores;

public class ElsaApiKeyStore(
    ISession session,
    ShellSettings shellSettings,
    ILogger<ElsaApiKeyStore> logger) : IApiKeyStore
{
    private const string Collection = ElsaAgentCollections.AgentApiKeys;

    public async Task AddAsync(ApiKeyDefinition entity, CancellationToken cancellationToken = default)
    {
        Prepare(entity);
        await session.SaveAsync(entity, Collection);
        await session.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ApiKeyDefinition entity, CancellationToken cancellationToken = default)
    {
        Prepare(entity);
        await session.SaveAsync(entity, Collection);
        await session.SaveChangesAsync(cancellationToken);
    }

    public async Task<ApiKeyDefinition?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        return await session.Query<ApiKeyDefinition, ApiKeyDefinitionIndex>(Collection)
            .Where(x => x.ApiKeyId == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ApiKeyDefinition?> FindAsync(ApiKeyDefinitionFilter filter, CancellationToken cancellationToken = default)
    {
        var index = await session.QueryIndex<ApiKeyDefinitionIndex>(Collection).Apply(filter).FirstOrDefaultAsync(cancellationToken);
        if (index == null)
            return null;

        return await GetAsync(index.ApiKeyId, cancellationToken);
    }

    public async Task<IEnumerable<ApiKeyDefinition>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await session.Query<ApiKeyDefinition, ApiKeyDefinitionIndex>(Collection).ListAsync(cancellationToken);
    }

    public async Task DeleteAsync(ApiKeyDefinition entity, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(entity.Id))
        {
            logger.LogWarning("Attempted to delete an API key without an identifier.");
            return;
        }

        var existing = await GetAsync(entity.Id, cancellationToken);
        if (existing == null)
            return;

        session.Delete(existing, Collection);
        await session.SaveChangesAsync(cancellationToken);
    }

    public async Task<long> DeleteManyAsync(ApiKeyDefinitionFilter filter, CancellationToken cancellationToken = default)
    {
        var indexes = await session.QueryIndex<ApiKeyDefinitionIndex>(Collection).Apply(filter).ListAsync(cancellationToken);
        long count = 0;

        foreach (var index in indexes)
        {
            var apiKey = await GetAsync(index.ApiKeyId, cancellationToken);
            if (apiKey == null)
                continue;

            session.Delete(apiKey, Collection);
            count++;
        }

        await session.SaveChangesAsync(cancellationToken);
        return count;
    }

    private void Prepare(ApiKeyDefinition entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Id))
            entity.Id = Guid.NewGuid().ToString("N");

        entity.TenantId ??= shellSettings.Name;
    }
}
