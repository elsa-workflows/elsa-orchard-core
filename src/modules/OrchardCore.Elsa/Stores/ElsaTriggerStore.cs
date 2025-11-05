using Elsa.Common.Entities;
using Elsa.Common.Models;
using Elsa.Workflows;
using Elsa.Workflows.Runtime;
using Elsa.Workflows.Runtime.Entities;
using Elsa.Workflows.Runtime.Filters;
using Elsa.Workflows.Runtime.OrderDefinitions;
using Open.Linq.AsyncExtensions;
using OrchardCore.Elsa.Documents;
using OrchardCore.Elsa.Indexes;
using OrchardCore.Elsa.Extensions;
using YesSql;

namespace OrchardCore.Elsa.Stores;

public class ElsaTriggerStore(ISession session, IPayloadSerializer payloadSerializer) : ITriggerStore
{
    private const string Collection = ElsaCollections.StoredTriggers;

    public async ValueTask SaveAsync(StoredTrigger record, CancellationToken cancellationToken = default)
    {
        var document = await Query(new() { Ids = new List<string> { record.Id } }).FirstOrDefaultAsync(cancellationToken);
        document = Map(document, record);
        await session.SaveAsync(document, Collection);
        await session.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask SaveManyAsync(IEnumerable<StoredTrigger> records, CancellationToken cancellationToken = default)
    {
        foreach (var record in records)
        {
            var document = await Query(new() { Ids = new List<string> { record.Id } }).FirstOrDefaultAsync(cancellationToken);
            document = Map(document, record);
            await session.SaveAsync(document, Collection);
        }

        await session.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask<StoredTrigger?> FindAsync(TriggerFilter filter, CancellationToken cancellationToken = default)
    {
        var document = await Query(filter).FirstOrDefaultAsync(cancellationToken);
        return Map(document);
    }

    public async ValueTask<IEnumerable<StoredTrigger>> FindManyAsync(TriggerFilter filter, CancellationToken cancellationToken = default)
    {
        var documents = await Query(filter).ListAsync(cancellationToken);
        return Map(documents);
    }

    public ValueTask<Page<StoredTrigger>> FindManyAsync(TriggerFilter filter, PageArgs pageArgs, CancellationToken cancellationToken = default)
    {
        return FindManyAsync(filter, pageArgs, new StoredTriggerOrder<string>(x => x.Id, OrderDirection.Ascending), cancellationToken);
    }

    public async ValueTask<Page<StoredTrigger>> FindManyAsync<TProp>(TriggerFilter filter, PageArgs pageArgs, StoredTriggerOrder<TProp> order, CancellationToken cancellationToken = default)
    {
        var query = Query(filter, order, pageArgs);
        var count = await query.CountAsync(cancellationToken);
        var documents = await query.ListAsync(cancellationToken).ToList();

        return Page.Of(Map(documents).ToList(), count);
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
            var query = Query(filter).OrderBy(x => x.TriggerId).Skip(pageArgs.Offset!.Value).Take(pageArgs.Limit!.Value);
            var documents = await query.ListAsync(cancellationToken).ToList();
            count += documents.Count;

            if (documents.Count == 0)
                break;

            foreach (var document in documents)
                session.Delete(document, Collection);

            pageArgs = pageArgs.Next();
        }

        await session.SaveChangesAsync(cancellationToken);
        return count;
    }

    private IQuery<StoredTriggerDocument, StoredTriggerIndex> Query(TriggerFilter filter)
    {
        return session.Query<StoredTriggerDocument, StoredTriggerIndex>(Collection).Apply(filter);
    }

    private IQuery<StoredTriggerDocument, StoredTriggerIndex> Query<TOrderBy>(TriggerFilter filter, StoredTriggerOrder<TOrderBy>? order = null, PageArgs? pageArgs = null)
    {
        var query = session.Query<StoredTriggerDocument, StoredTriggerIndex>(Collection).Apply(filter);
        if (order != null) query = query.Apply(order);

        if (pageArgs != null)
        {
            if (pageArgs.Offset != null) query.Skip(pageArgs.Offset.Value);
            if (pageArgs.Limit != null) query.Take(pageArgs.Limit.Value);
        }

        return query;
    }

    private StoredTriggerDocument Map(StoredTriggerDocument? target, StoredTrigger source)
    {
        if (target == null)
            target = new();

        target.TriggerId = source.Id;
        target.TenantId = source.TenantId;
        target.WorkflowDefinitionId = source.WorkflowDefinitionId;
        target.WorkflowDefinitionVersionId = source.WorkflowDefinitionVersionId;
        target.Name = source.Name;
        target.ActivityId = source.ActivityId;
        target.Hash = source.Hash;

        if (source.Payload != null)
            target.SerializedPayload = payloadSerializer.Serialize(source.Payload);

        return target;
    }

    private IEnumerable<StoredTrigger> Map(IEnumerable<StoredTriggerDocument> source)
    {
        return source.Select(x => Map(x)!);
    }

    private StoredTrigger? Map(StoredTriggerDocument? source)
    {
        if (source == null)
            return null;

        var payload = source.SerializedPayload != null
            ? payloadSerializer.Deserialize(source.SerializedPayload)
            : null;

        return new()
        {
            Id = source.TriggerId,
            TenantId = source.TenantId,
            WorkflowDefinitionId = source.WorkflowDefinitionId,
            WorkflowDefinitionVersionId = source.WorkflowDefinitionVersionId,
            Name = source.Name,
            ActivityId = source.ActivityId,
            Hash = source.Hash,
            Payload = payload
        };
    }
}
