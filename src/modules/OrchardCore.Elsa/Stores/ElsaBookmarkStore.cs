using Elsa.Common.Models;
using Elsa.Workflows;
using Elsa.Workflows.Runtime;
using Elsa.Workflows.Runtime.Entities;
using Elsa.Workflows.Runtime.Filters;
using Open.Linq.AsyncExtensions;
using OrchardCore.Elsa.Documents;
using OrchardCore.Elsa.Indexes;
using OrchardCore.Elsa.Extensions;
using YesSql;

namespace OrchardCore.Elsa.Stores;

public class ElsaBookmarkStore(ISession session, IPayloadSerializer payloadSerializer) : IBookmarkStore
{
    private const string Collection = ElsaCollections.StoredBookmarks;

    public async ValueTask SaveAsync(StoredBookmark record, CancellationToken cancellationToken = default)
    {
        var document = await Query(new() { BookmarkId = record.Id }).FirstOrDefaultAsync(cancellationToken);
        document = Map(document, record);
        await session.SaveAsync(document, Collection);
        await session.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask SaveManyAsync(IEnumerable<StoredBookmark> records, CancellationToken cancellationToken)
    {
        foreach (var record in records)
        {
            var document = await Query(new() { BookmarkId = record.Id }).FirstOrDefaultAsync(cancellationToken);
            document = Map(document, record);
            await session.SaveAsync(document, Collection);
        }

        await session.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask<StoredBookmark?> FindAsync(BookmarkFilter filter, CancellationToken cancellationToken = default)
    {
        var document = await Query(filter).FirstOrDefaultAsync(cancellationToken);
        return Map(document);
    }

    public async ValueTask<IEnumerable<StoredBookmark>> FindManyAsync(BookmarkFilter filter, CancellationToken cancellationToken = default)
    {
        var documents = await Query(filter).ListAsync(cancellationToken);
        return Map(documents);
    }

    public async ValueTask<long> DeleteAsync(BookmarkFilter filter, CancellationToken cancellationToken = default)
    {
        var pageArgs = PageArgs.FromRange(0, 100);
        var count = 0;

        while (true)
        {
            var query = Query(filter).OrderBy(x => x.BookmarkId).Skip(pageArgs.Offset!.Value).Take(pageArgs.Limit!.Value);
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

    private IQuery<StoredBookmarkDocument, StoredBookmarkIndex> Query(BookmarkFilter filter)
    {
        return session.Query<StoredBookmarkDocument, StoredBookmarkIndex>(Collection).Apply(filter);
    }

    private StoredBookmarkDocument Map(StoredBookmarkDocument? target, StoredBookmark source)
    {
        if (target == null)
            target = new();

        target.BookmarkId = source.Id;
        target.TenantId = source.TenantId;
        target.Name = source.Name;
        target.Hash = source.Hash;
        target.WorkflowInstanceId = source.WorkflowInstanceId;
        target.ActivityInstanceId = source.ActivityInstanceId;
        target.CorrelationId = source.CorrelationId;
        target.CreatedAt = source.CreatedAt;

        if (source.Payload != null)
            target.SerializedPayload = payloadSerializer.Serialize(source.Payload);

        if (source.Metadata != null)
            target.SerializedMetadata = payloadSerializer.Serialize(source.Metadata);

        return target;
    }

    private IEnumerable<StoredBookmark> Map(IEnumerable<StoredBookmarkDocument> source)
    {
        return source.Select(x => Map(x)!);
    }

    private StoredBookmark? Map(StoredBookmarkDocument? source)
    {
        if (source == null)
            return null;

        var payload = source.SerializedPayload != null
            ? payloadSerializer.Deserialize(source.SerializedPayload)
            : null;

        var metadata = source.SerializedMetadata != null
            ? payloadSerializer.Deserialize<Dictionary<string, string>>(source.SerializedMetadata)
            : null;

        return new()
        {
            Id = source.BookmarkId,
            TenantId = source.TenantId,
            Name = source.Name,
            Hash = source.Hash,
            WorkflowInstanceId = source.WorkflowInstanceId,
            ActivityInstanceId = source.ActivityInstanceId,
            CorrelationId = source.CorrelationId,
            CreatedAt = source.CreatedAt,
            Payload = payload,
            Metadata = metadata
        };
    }
}