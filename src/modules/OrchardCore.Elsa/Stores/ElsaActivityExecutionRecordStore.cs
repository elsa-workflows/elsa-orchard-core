using Elsa.Common.Models;
using Elsa.Workflows.Runtime;
using Elsa.Workflows.Runtime.Entities;
using Elsa.Workflows.Runtime.Filters;
using Elsa.Workflows.Runtime.OrderDefinitions;
using Open.Linq.AsyncExtensions;
using OrchardCore.Elsa.Indexes;
using OrchardCore.Elsa.Extensions;
using YesSql;

namespace OrchardCore.Elsa.Stores;

public class ElsaActivityExecutionRecordStore(ISession session) : IActivityExecutionStore
{
    private const string Collection = ElsaCollections.ActivityExecutionRecords;

    public async Task SaveManyAsync(IEnumerable<ActivityExecutionRecord> records, CancellationToken cancellationToken = default)
    {
        foreach (var record in records)
            await session.SaveAsync(record, Collection);

        await session.FlushAsync(cancellationToken);
    }

    public Task AddManyAsync(IEnumerable<ActivityExecutionRecord> records, CancellationToken cancellationToken = default)
    {
        return SaveManyAsync(records, cancellationToken);
    }

    public async Task SaveAsync(ActivityExecutionRecord record, CancellationToken cancellationToken = default)
    {
        var recordToSave = await Query(new() { Id = record.Id }).FirstOrDefaultAsync(cancellationToken);
        if (recordToSave != null)
        {
            record.Id = recordToSave.Id;
            record.ActivityId = recordToSave.ActivityId;
            record.ActivityType = recordToSave.ActivityType;
            record.ActivityName = recordToSave.ActivityName;
            record.ActivityTypeVersion = recordToSave.ActivityTypeVersion;
            record.ActivityNodeId = recordToSave.ActivityNodeId;
            record.WorkflowInstanceId = recordToSave.WorkflowInstanceId;
            record.Status = recordToSave.Status;
            record.StartedAt = recordToSave.StartedAt;
            record.CompletedAt = recordToSave.CompletedAt;
            record.HasBookmarks = recordToSave.HasBookmarks;
            record.TenantId = recordToSave.TenantId;
            record.Payload = recordToSave.Payload;
            record.Exception = recordToSave.Exception;
            record.ActivityState = recordToSave.ActivityState;
            record.AggregateFaultCount = recordToSave.AggregateFaultCount;
            record.Metadata = recordToSave.Metadata;
            record.Outputs = recordToSave.Outputs;
            record.Properties = recordToSave.Properties;
            record.SerializedSnapshot = recordToSave.SerializedSnapshot;
        }
        else
        {
            recordToSave = record;
        }
        
        await session.SaveAsync(recordToSave, Collection);
        await session.FlushAsync(cancellationToken);
    }

    public async Task<ActivityExecutionRecord?> FindAsync(ActivityExecutionRecordFilter filter, CancellationToken cancellationToken = default)
    {
        var result = await Query(filter).FirstOrDefaultAsync(cancellationToken);
        return result;
    }

    public async Task<IEnumerable<ActivityExecutionRecord>> FindManyAsync<TOrderBy>(ActivityExecutionRecordFilter filter, ActivityExecutionRecordOrder<TOrderBy> order, CancellationToken cancellationToken = default)
    {
        var results = await Query(filter, order).ListAsync(cancellationToken);
        return results;
    }

    public async Task<IEnumerable<ActivityExecutionRecord>> FindManyAsync(ActivityExecutionRecordFilter filter, CancellationToken cancellationToken = default)
    {
        var results = await Query(filter).ListAsync(cancellationToken);
        return results;
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

    private IQuery<ActivityExecutionRecord, ActivityExecutionRecordIndex> Query(ActivityExecutionRecordFilter filter, PageArgs? pageArgs = null)
    {
        return Query<string>(filter, pageArgs: pageArgs);
    }

    private IQuery<ActivityExecutionRecord, ActivityExecutionRecordIndex> Query<TOrderBy>(ActivityExecutionRecordFilter filter, ActivityExecutionRecordOrder<TOrderBy>? order = null, PageArgs? pageArgs = null)
    {
        var query = session.Query<ActivityExecutionRecord, ActivityExecutionRecordIndex>(Collection).Apply(filter);
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
}