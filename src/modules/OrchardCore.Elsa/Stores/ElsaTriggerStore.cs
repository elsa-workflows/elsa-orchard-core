using Elsa.Common.Entities;
using Elsa.Common.Models;
using Elsa.Workflows;
using Elsa.Workflows.Runtime;
using Elsa.Workflows.Runtime.Entities;
using Elsa.Workflows.Runtime.Filters;
using Elsa.Workflows.Runtime.OrderDefinitions;
using Open.Linq.AsyncExtensions;
using OrchardCore.Elsa.Indexes;
using OrchardCore.Elsa.Extensions;
using YesSql;

namespace OrchardCore.Elsa.Stores;

public class ElsaTriggerStore(ISession session, IPayloadSerializer payloadSerializer) : ITriggerStore
{
    private const string Collection = ElsaCollections.StoredTriggers;

    public async ValueTask SaveAsync(StoredTrigger record, CancellationToken cancellationToken = default)
    {
        var serializedRecord = OnSave(record);
        await session.SaveAsync(serializedRecord, Collection);
        await session.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask SaveManyAsync(IEnumerable<StoredTrigger> records, CancellationToken cancellationToken = default)
    {
        foreach (var record in records)
        {
            var serializedRecord = OnSave(record);
            await session.SaveAsync(serializedRecord, Collection);
        }

        await session.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask<StoredTrigger?> FindAsync(TriggerFilter filter, CancellationToken cancellationToken = default)
    {
        var record = await Query(filter).FirstOrDefaultAsync(cancellationToken);
        return OnLoad(record);
    }

    public async ValueTask<IEnumerable<StoredTrigger>> FindManyAsync(TriggerFilter filter, CancellationToken cancellationToken = default)
    {
        var records = await Query(filter).ListAsync(cancellationToken);
        return OnLoad(records);
    }

    public ValueTask<Page<StoredTrigger>> FindManyAsync(TriggerFilter filter, PageArgs pageArgs, CancellationToken cancellationToken = default)
    {
        return FindManyAsync(filter, pageArgs, new StoredTriggerOrder<string>(x => x.Id, OrderDirection.Ascending), cancellationToken);
    }

    public async ValueTask<Page<StoredTrigger>> FindManyAsync<TProp>(TriggerFilter filter, PageArgs pageArgs, StoredTriggerOrder<TProp> order, CancellationToken cancellationToken = default)
    {
        var query = Query(filter, order, pageArgs);
        var count = await query.CountAsync(cancellationToken);
        var records = await query.ListAsync(cancellationToken).ToList();

        return Page.Of(OnLoad(records).ToList(), count);
    }

    public async ValueTask ReplaceAsync(IEnumerable<StoredTrigger> removed, IEnumerable<StoredTrigger> added, CancellationToken cancellationToken = default)
    {
        var removedList = removed.ToList();

        if (removedList.Count > 0)
        {
            var filter = new TriggerFilter { Ids = removedList.Select(r => r.Id).ToList() };
            await DeleteManyAsync(filter, cancellationToken);
        }

        await SaveManyAsync(added, cancellationToken);
    }

    public async ValueTask<long> DeleteManyAsync(TriggerFilter filter, CancellationToken cancellationToken = default)
    {
        var pageArgs = PageArgs.FromRange(0, 100);
        var count = 0;

        while (true)
        {
            var query = Query(filter).OrderBy(x => x.Id).Skip(pageArgs.Offset!.Value).Take(pageArgs.Limit!.Value);
            var records = await query.ListAsync(cancellationToken).ToList();
            count += records.Count;

            if (records.Count == 0)
                break;

            foreach (var record in records)
                session.Delete(record, Collection);

            pageArgs = pageArgs.Next();
        }

        return count;
    }

    private IQuery<StoredTrigger, StoredTriggerIndex> Query(TriggerFilter filter)
    {
        return session.Query<StoredTrigger, StoredTriggerIndex>(Collection).Apply(filter);
    }

    private IQuery<StoredTrigger, StoredTriggerIndex> Query<TOrderBy>(TriggerFilter filter, StoredTriggerOrder<TOrderBy>? order = null, PageArgs? pageArgs = null)
    {
        var query = session.Query<StoredTrigger, StoredTriggerIndex>(Collection).Apply(filter);
        if (order != null) query = query.Apply(order);

        if (pageArgs != null)
        {
            if (pageArgs.Offset != null) query.Skip(pageArgs.Offset.Value);
            if (pageArgs.Limit != null) query.Take(pageArgs.Limit.Value);
        }

        return query;
    }

    private StoredTrigger OnSave(StoredTrigger record)
    {
        var serializedRecord = new StoredTrigger
        {
            ActivityId = record.ActivityId,
            Payload = record.Payload,
            Hash = record.Hash,
            Id = record.Id,
            Name = record.Name,
            TenantId = record.TenantId,
            WorkflowDefinitionId = record.WorkflowDefinitionId,
            WorkflowDefinitionVersionId = record.WorkflowDefinitionVersionId
        };

        if (serializedRecord.Payload != null)
            serializedRecord.Payload = payloadSerializer.Serialize(serializedRecord.Payload);

        return serializedRecord;
    }

    private IEnumerable<StoredTrigger> OnLoad(IEnumerable<StoredTrigger> records)
    {
        return records.Select(record => OnLoad(record)!);
    }

    private StoredTrigger? OnLoad(StoredTrigger? record)
    {
        if (record?.Payload is string serializedPayload)
            record.Payload = payloadSerializer.Deserialize(serializedPayload);

        return record;
    }
}