using Elsa.Agents.Persistence.Contracts;
using Elsa.Agents.Persistence.Entities;
using Elsa.Agents.Persistence.Filters;
using Microsoft.Extensions.Logging;
using OrchardCore.Elsa.Agents.Extensions;
using OrchardCore.Elsa.Agents.Indexes;
using OrchardCore.Environment.Shell;
using YesSql;

namespace OrchardCore.Elsa.Agents.Stores;

public class ElsaServiceStore(
    ISession session,
    ShellSettings shellSettings,
    ILogger<ElsaServiceStore> logger) : IServiceStore
{
    private const string Collection = ElsaAgentCollections.AgentServices;

    public async Task AddAsync(ServiceDefinition entity, CancellationToken cancellationToken = default)
    {
        Prepare(entity);
        await session.SaveAsync(entity, Collection);
        await session.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ServiceDefinition entity, CancellationToken cancellationToken = default)
    {
        Prepare(entity);
        await session.SaveAsync(entity, Collection);
        await session.SaveChangesAsync(cancellationToken);
    }

    public async Task<ServiceDefinition?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        return await session.Query<ServiceDefinition, ServiceDefinitionIndex>(Collection)
            .Where(x => x.ServiceId == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ServiceDefinition?> FindAsync(ServiceDefinitionFilter filter, CancellationToken cancellationToken = default)
    {
        var index = await session.QueryIndex<ServiceDefinitionIndex>(Collection).Apply(filter).FirstOrDefaultAsync(cancellationToken);
        if (index == null)
            return null;

        return await GetAsync(index.ServiceId, cancellationToken);
    }

    public async Task<IEnumerable<ServiceDefinition>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await session.Query<ServiceDefinition, ServiceDefinitionIndex>(Collection).ListAsync(cancellationToken);
    }

    public async Task DeleteAsync(ServiceDefinition entity, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(entity.Id))
        {
            logger.LogWarning("Attempted to delete a service without an identifier.");
            return;
        }

        var existing = await GetAsync(entity.Id, cancellationToken);
        if (existing == null)
            return;

        session.Delete(existing, Collection);
        await session.SaveChangesAsync(cancellationToken);
    }

    public async Task<long> DeleteManyAsync(ServiceDefinitionFilter filter, CancellationToken cancellationToken = default)
    {
        var indexes = await session.QueryIndex<ServiceDefinitionIndex>(Collection).Apply(filter).ListAsync(cancellationToken);
        long count = 0;

        foreach (var index in indexes)
        {
            var service = await GetAsync(index.ServiceId, cancellationToken);
            if (service == null)
                continue;

            session.Delete(service, Collection);
            count++;
        }

        await session.SaveChangesAsync(cancellationToken);
        return count;
    }

    private void Prepare(ServiceDefinition entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Id))
            entity.Id = Guid.NewGuid().ToString("N");

        entity.TenantId ??= shellSettings.Name;
    }
}
