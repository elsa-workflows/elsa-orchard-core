using System.Text.Json;
using Elsa.Common.Models;
using Elsa.Workflows;
using Elsa.Workflows.Runtime;
using Elsa.Workflows.Runtime.Entities;
using Elsa.Workflows.Runtime.Filters;
using Elsa.Workflows.Runtime.OrderDefinitions;
using Elsa.Workflows.State;
using Open.Linq.AsyncExtensions;
using OrchardCore.Elsa.Documents;
using OrchardCore.Elsa.Indexes;
using OrchardCore.Elsa.Extensions;
using YesSql;

namespace OrchardCore.Elsa.Stores;

public class ElsaActivityExecutionRecordStore(ISession session, IPayloadSerializer payloadSerializer) : IActivityExecutionStore
{
    private const string Collection = ElsaCollections.ActivityExecutionRecords;

    public async Task SaveManyAsync(IEnumerable<ActivityExecutionRecord> records, CancellationToken cancellationToken = default)
    {
        foreach (var record in records)
        {
            var existingRecord = await Query(new() { Id = record.Id }).FirstOrDefaultAsync(cancellationToken);
            existingRecord = Map(existingRecord, record);
            await session.SaveAsync(existingRecord, Collection);
        }

        await session.SaveChangesAsync(cancellationToken);
    }

    public async Task AddManyAsync(IEnumerable<ActivityExecutionRecord> records, CancellationToken cancellationToken = default)
    {
        foreach (var record in records)
        {
            var document = Map(new(), record);
            await session.SaveAsync(document, Collection);
        }

        await session.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveAsync(ActivityExecutionRecord record, CancellationToken cancellationToken = default)
    {
        var document = await Query(new() { Id = record.Id }).FirstOrDefaultAsync(cancellationToken);
        document = Map(document, record);
        await session.SaveAsync(document, Collection);
        await session.SaveChangesAsync(cancellationToken);
    }

    public async Task<ActivityExecutionRecord?> FindAsync(ActivityExecutionRecordFilter filter, CancellationToken cancellationToken = default)
    {
        var result = await Query(filter).FirstOrDefaultAsync(cancellationToken);
        return Map(result);
    }

    public async Task<IEnumerable<ActivityExecutionRecord>> FindManyAsync<TOrderBy>(ActivityExecutionRecordFilter filter, ActivityExecutionRecordOrder<TOrderBy> order, CancellationToken cancellationToken = default)
    {
        var results = await Query(filter, order).ListAsync(cancellationToken);
        return Map(results);
    }

    public async Task<IEnumerable<ActivityExecutionRecord>> FindManyAsync(ActivityExecutionRecordFilter filter, CancellationToken cancellationToken = default)
    {
        var results = await Query(filter).ListAsync(cancellationToken);
        return Map(results);
    }

    public async Task<IEnumerable<ActivityExecutionRecordSummary>> FindManySummariesAsync<TOrderBy>(ActivityExecutionRecordFilter filter, ActivityExecutionRecordOrder<TOrderBy> order, CancellationToken cancellationToken = default)
    {
        var query = QueryIndex(filter, order);
        var indexes = await query.ListAsync(cancellationToken).ToList();
        return MapSummaries(indexes).ToList();
    }

    public async Task<IEnumerable<ActivityExecutionRecordSummary>> FindManySummariesAsync(ActivityExecutionRecordFilter filter, CancellationToken cancellationToken = default)
    {
        var query = QueryIndex(filter);
        var indexes = await query.ListAsync(cancellationToken).ToList();
        return MapSummaries(indexes).ToList();
    }

    public async Task<long> CountAsync(ActivityExecutionRecordFilter filter, CancellationToken cancellationToken = default)
    {
        return await QueryIndex(filter).CountAsync(cancellationToken);
    }

    public async Task<long> DeleteManyAsync(ActivityExecutionRecordFilter filter, CancellationToken cancellationToken = default)
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

    private IQuery<ActivityExecutionRecordDocument, ActivityExecutionRecordIndex> Query(ActivityExecutionRecordFilter filter, PageArgs? pageArgs = null)
    {
        return Query<string>(filter, pageArgs: pageArgs);
    }

    private IQuery<ActivityExecutionRecordDocument, ActivityExecutionRecordIndex> Query<TOrderBy>(ActivityExecutionRecordFilter filter, ActivityExecutionRecordOrder<TOrderBy>? order = null, PageArgs? pageArgs = null)
    {
        var query = session.Query<ActivityExecutionRecordDocument, ActivityExecutionRecordIndex>(Collection).Apply(filter);
        if (order != null) query = query.Apply(order);

        if (pageArgs != null)
        {
            if (pageArgs.Offset != null) query.Skip(pageArgs.Offset.Value);
            if (pageArgs.Limit != null) query.Take(pageArgs.Limit.Value);
        }

        return query;
    }

    private IQueryIndex<ActivityExecutionRecordIndex> QueryIndex(ActivityExecutionRecordFilter filter, PageArgs? pageArgs = null)
    {
        return QueryIndex<string>(filter, null, pageArgs);
    }

    private IQueryIndex<ActivityExecutionRecordIndex> QueryIndex<TOrderBy>(ActivityExecutionRecordFilter filter, ActivityExecutionRecordOrder<TOrderBy>? order = null, PageArgs? pageArgs = null)
    {
        var query = session.QueryIndex<ActivityExecutionRecordIndex>(Collection).Apply(filter);
        if (order != null) query = query.Apply(order);

        if (pageArgs != null)
        {
            if (pageArgs.Offset != null) query.Skip(pageArgs.Offset.Value);
            if (pageArgs.Limit != null) query.Take(pageArgs.Limit.Value);
        }

        return query;
    }
    
    private IEnumerable<ActivityExecutionRecordSummary> MapSummaries(IEnumerable<ActivityExecutionRecordIndex> indexes)
    {
        return indexes.Select(MapSummary);
    }

    private ActivityExecutionRecordSummary MapSummary(ActivityExecutionRecordIndex index)
    {
        return new()
        {
            Id = index.RecordId,
            ActivityId = index.ActivityId,
            StartedAt = index.StartedAt,
            ActivityType = index.ActivityType,
            CompletedAt = index.CompletedAt,
            WorkflowInstanceId = index.WorkflowInstanceId,
            ActivityName = index.ActivityName,
            HasBookmarks = index.HasBookmarks,
            ActivityNodeId = index.ActivityNodeId,
            ActivityTypeVersion = index.ActivityTypeVersion,
            Status = index.Status
        };
    }

    private ActivityExecutionRecordDocument Map(ActivityExecutionRecordDocument? target, ActivityExecutionRecord source)
    {
        if (target == null)
            target = new();

        target.RecordId = source.Id;
        target.ActivityId = source.ActivityId;
        target.ActivityType = source.ActivityType;
        target.ActivityName = source.ActivityName;
        target.ActivityTypeVersion = source.ActivityTypeVersion;
        target.ActivityNodeId = source.ActivityNodeId;
        target.WorkflowInstanceId = source.WorkflowInstanceId;
        target.Status = source.Status;
        target.StartedAt = source.StartedAt;
        target.CompletedAt = source.CompletedAt;
        target.HasBookmarks = source.HasBookmarks;
        target.TenantId = source.TenantId;
        target.AggregateFaultCount = source.AggregateFaultCount;

        var snapshot = source.SerializedSnapshot;

        if (snapshot is not null)
        {
            target.SerializedActivityState = snapshot.SerializedActivityState;
            target.SerializedOutputs = snapshot.SerializedOutputs;
            target.SerializedProperties = snapshot.SerializedProperties;
            target.SerializedMetadata = snapshot.SerializedMetadata;
            target.SerializedException = snapshot.SerializedException;
            target.SerializedPayload = snapshot.SerializedPayload;
        }

        return target;
    }

    private IEnumerable<ActivityExecutionRecord> Map(IEnumerable<ActivityExecutionRecordDocument> source)
    {
        return source.Select(x => Map(x)!);
    }

    private ActivityExecutionRecord? Map(ActivityExecutionRecordDocument? source)
    {
        if (source == null)
            return null;

        var record = new ActivityExecutionRecord
        {
            Id = source.RecordId,
            ActivityId = source.ActivityId,
            ActivityType = source.ActivityType,
            ActivityName = source.ActivityName,
            ActivityTypeVersion = source.ActivityTypeVersion,
            ActivityNodeId = source.ActivityNodeId,
            WorkflowInstanceId = source.WorkflowInstanceId,
            Status = source.Status,
            StartedAt = source.StartedAt,
            CompletedAt = source.CompletedAt,
            HasBookmarks = source.HasBookmarks,
            TenantId = source.TenantId,
            AggregateFaultCount = source.AggregateFaultCount,
            ActivityState = DeserializeActivityState(source.SerializedActivityState),
            Outputs = Deserialize<IDictionary<string, object?>>(source.SerializedOutputs),
            Properties = DeserializePayload<IDictionary<string, object>>(source.SerializedProperties),
            Metadata = DeserializePayload<IDictionary<string, object>>(source.SerializedMetadata),
            Exception = DeserializePayload<ExceptionState>(source.SerializedException),
            Payload = DeserializePayload<IDictionary<string, object>>(source.SerializedPayload)
        };

        return record;
    }

    private IDictionary<string, object?>? DeserializeActivityState(string? json)
    {
        if (!string.IsNullOrWhiteSpace(json))
        {
            var dictionary = JsonSerializer.Deserialize<IDictionary<string, object?>>(json);
            return dictionary?.ToDictionary(x => x.Key, x => x.Value);
        }

        return null;
    }

    private T? Deserialize<T>(string? json)
    {
        return !string.IsNullOrEmpty(json) ? JsonSerializer.Deserialize<T>(json) : default;
    }

    private T? DeserializePayload<T>(string? json)
    {
        return !string.IsNullOrEmpty(json) ? payloadSerializer.Deserialize<T>(json) : default;
    }
}