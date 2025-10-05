using Elsa.Common.Models;
using Elsa.Workflows;
using Elsa.Workflows.Runtime;
using Elsa.Workflows.Runtime.Entities;
using Elsa.Workflows.Runtime.Filters;
using Open.Linq.AsyncExtensions;
using OrchardCore.Elsa.Indexes;
using OrchardCore.Elsa.Extensions;
using YesSql;

namespace OrchardCore.Elsa.Stores;

public class ElsaBookmarkStore(ISession session, IPayloadSerializer payloadSerializer) : IBookmarkStore
{
    private const string Collection = ElsaCollections.StoredBookmarks;

    public async ValueTask SaveAsync(StoredBookmark record, CancellationToken cancellationToken = default)
    {
        var existingRecord = await Query(new() { BookmarkId = record.Id }).FirstOrDefaultAsync(cancellationToken);
        existingRecord = MapRecord(existingRecord, record);
        var serializedRecord = OnSave(existingRecord);
        await session.SaveAsync(serializedRecord, Collection);
        await session.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask SaveManyAsync(IEnumerable<StoredBookmark> records, CancellationToken cancellationToken)
    {
        foreach (var record in records)
        {
            var existingRecord = await Query(new() { BookmarkId = record.Id }).FirstOrDefaultAsync(cancellationToken);
            existingRecord = MapRecord(existingRecord, record);
            var serializedRecord = OnSave(existingRecord);
            await session.SaveAsync(serializedRecord, Collection);
        }

        await session.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask<StoredBookmark?> FindAsync(BookmarkFilter filter, CancellationToken cancellationToken = default)
    {
        var record = await Query(filter).FirstOrDefaultAsync(cancellationToken);
        return OnLoad(record);
    }

    public async ValueTask<IEnumerable<StoredBookmark>> FindManyAsync(BookmarkFilter filter, CancellationToken cancellationToken = default)
    {
        var records = await Query(filter).ListAsync(cancellationToken);
        return OnLoad(records);
    }

    public async ValueTask<long> DeleteAsync(BookmarkFilter filter, CancellationToken cancellationToken = default)
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

        await session.SaveChangesAsync(cancellationToken);
        return count;
    }

    private IQuery<StoredBookmark, StoredBookmarkIndex> Query(BookmarkFilter filter)
    {
        return session.Query<StoredBookmark, StoredBookmarkIndex>(Collection).Apply(filter);
    }

    private StoredBookmark OnSave(StoredBookmark record)
    {
        var serializedRecord = new StoredBookmark
        {
            Payload = record.Payload,
            Hash = record.Hash,
            Id = record.Id,
            Name = record.Name,
            TenantId = record.TenantId,
            WorkflowInstanceId = record.WorkflowInstanceId,
            ActivityInstanceId = record.ActivityInstanceId,
            CorrelationId = record.CorrelationId,
            CreatedAt = record.CreatedAt,
            Metadata = record.Metadata,
            ActivityTypeName = record.ActivityTypeName
        };

        if (serializedRecord.Payload != null)
            serializedRecord.Payload = payloadSerializer.Serialize(serializedRecord.Payload);

        return serializedRecord;
    }

    private IEnumerable<StoredBookmark> OnLoad(IEnumerable<StoredBookmark> records)
    {
        return records.Select(record => OnLoad(record)!);
    }

    private StoredBookmark? OnLoad(StoredBookmark? record)
    {
        if (record?.Payload is string serializedPayload)
            record.Payload = payloadSerializer.Deserialize(serializedPayload);

        return record;
    }

    private StoredBookmark MapRecord(StoredBookmark? target, StoredBookmark source)
    {
        if (target == null)
            return source;

        target.Payload = source.Payload;
        target.Hash = source.Hash;
        target.Id = source.Id;
        target.Name = source.Name;
        target.TenantId = source.TenantId;
        target.WorkflowInstanceId = source.WorkflowInstanceId;
        target.ActivityInstanceId = source.ActivityInstanceId;
        target.CorrelationId = source.CorrelationId;
        target.CreatedAt = source.CreatedAt;
        target.Metadata = source.Metadata;
        target.ActivityTypeName = source.ActivityTypeName;
        return target;
    }
}