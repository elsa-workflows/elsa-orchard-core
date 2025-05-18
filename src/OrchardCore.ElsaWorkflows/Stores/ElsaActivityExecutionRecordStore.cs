using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Common.Models;
using Elsa.Workflows.Runtime;
using Elsa.Workflows.Runtime.Entities;
using Elsa.Workflows.Runtime.Filters;
using Elsa.Workflows.Runtime.OrderDefinitions;
using Open.Linq.AsyncExtensions;
using OrchardCore.ElsaWorkflows.Extensions;
using OrchardCore.ElsaWorkflows.Indexes;
using YesSql;

namespace OrchardCore.ElsaWorkflows.Stores;

public class ElsaActivityExecutionRecordStore(ISession session) : IActivityExecutionStore
{
    private const string Collection = ElsaCollections.ActivityExecutionRecords;

    public async Task SaveManyAsync(IEnumerable<ActivityExecutionRecord> records, CancellationToken cancellationToken = default)
    {
        foreach (var record in records)
            await session.SaveAsync(record, Collection);

        await session.FlushAsync();
    }

    public Task AddManyAsync(IEnumerable<ActivityExecutionRecord> records, CancellationToken cancellationToken = new CancellationToken())
    {
        return SaveManyAsync(records, cancellationToken);
    }

    public async Task SaveAsync(ActivityExecutionRecord record, CancellationToken cancellationToken = default)
    {
        await session.SaveAsync(record, Collection);
        await session.FlushAsync();
    }

    public async Task<ActivityExecutionRecord?> FindAsync(ActivityExecutionRecordFilter filter, CancellationToken cancellationToken = default)
    {
        return await Query(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ActivityExecutionRecord>> FindManyAsync<TOrderBy>(ActivityExecutionRecordFilter filter, ActivityExecutionRecordOrder<TOrderBy> order, CancellationToken cancellationToken = default)
    {
        return await Query(filter, order).ListAsync();
    }

    public async Task<IEnumerable<ActivityExecutionRecord>> FindManyAsync(ActivityExecutionRecordFilter filter, CancellationToken cancellationToken = default)
    {
        return await Query(filter).ListAsync();
    }

    public async Task<IEnumerable<ActivityExecutionRecordSummary>> FindManySummariesAsync<TOrderBy>(ActivityExecutionRecordFilter filter, ActivityExecutionRecordOrder<TOrderBy> order, CancellationToken cancellationToken = default)
    {
        var query = QueryIndex(filter, order);
        var indexes = await query.ListAsync().ToList();
        return MapSummaries(indexes).ToList();
    }

    public async Task<IEnumerable<ActivityExecutionRecordSummary>> FindManySummariesAsync(ActivityExecutionRecordFilter filter, CancellationToken cancellationToken = default)
    {
        var query = QueryIndex(filter);
        var indexes = await query.ListAsync().ToList();
        return MapSummaries(indexes).ToList();
    }

    public async Task<long> CountAsync(ActivityExecutionRecordFilter filter, CancellationToken cancellationToken = default)
    {
        return await QueryIndex(filter).CountAsync();
    }

    public async Task<long> DeleteManyAsync(ActivityExecutionRecordFilter filter, CancellationToken cancellationToken = default)
    {
        var pageArgs = PageArgs.FromRange(0, 100);
        var count = 0;
        
        while (true)
        {
            var query = Query(filter).OrderBy(x => x.Id).Skip(pageArgs.Offset!.Value).Take(pageArgs.Limit!.Value);
            var records = await query.ListAsync().ToList();
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