using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Common.Entities;
using Elsa.Common.Models;
using Elsa.Workflows.Management;
using Elsa.Workflows.Management.Entities;
using Elsa.Workflows.Management.Filters;
using Elsa.Workflows.Management.Models;
using Open.Linq.AsyncExtensions;
using OrchardCore.ElsaWorkflows.Extensions;
using OrchardCore.ElsaWorkflows.Indexes;
using YesSql;

namespace OrchardCore.ElsaWorkflows.Stores;

public class ElsaWorkflowInstanceStore(ISession session) : IWorkflowInstanceStore
{
    public async ValueTask<WorkflowInstance?> FindAsync(WorkflowInstanceFilter filter, CancellationToken cancellationToken = default)
    {
        return await Query(filter).FirstOrDefaultAsync();
    }

    public async ValueTask<Page<WorkflowInstance>> FindManyAsync(WorkflowInstanceFilter filter, PageArgs pageArgs, CancellationToken cancellationToken = default)
    {
        var query = Query(filter, pageArgs);
        var count = await query.CountAsync();
        var records = await query.ListAsync().ToList();

        return Page.Of(records, count);
    }

    public async ValueTask<Page<WorkflowInstance>> FindManyAsync<TOrderBy>(WorkflowInstanceFilter filter, PageArgs pageArgs, WorkflowInstanceOrder<TOrderBy> order, CancellationToken cancellationToken = default)
    {
        var query = Query(filter, order, pageArgs);
        var count = await query.CountAsync();
        var records = await query.ListAsync().ToList();

        return Page.Of(records, count);
    }

    public async ValueTask<IEnumerable<WorkflowInstance>> FindManyAsync(WorkflowInstanceFilter filter, CancellationToken cancellationToken = default)
    {
        return await Query(filter).ListAsync();
    }

    public async ValueTask<IEnumerable<WorkflowInstance>> FindManyAsync<TOrderBy>(WorkflowInstanceFilter filter, WorkflowInstanceOrder<TOrderBy> order, CancellationToken cancellationToken = default)
    {
        return await Query(filter, order).ListAsync();
    }

    public async ValueTask<long> CountAsync(WorkflowInstanceFilter filter, CancellationToken cancellationToken = default)
    {
        return await Query(filter).CountAsync();
    }

    public async ValueTask<IEnumerable<string>> FindManyIdsAsync(WorkflowInstanceFilter filter, CancellationToken cancellationToken = default)
    {
        var query = QueryIndex(filter);
        var indexes = await query.ListAsync().ToList();
        return indexes.Select(x => x.InstanceId);
    }

    public async ValueTask<Page<string>> FindManyIdsAsync(WorkflowInstanceFilter filter, PageArgs pageArgs, CancellationToken cancellationToken = default)
    {
        var query = QueryIndex(filter, pageArgs);
        var count = await query.CountAsync();
        var indexes = await query.ListAsync().ToList();
        var instanceIds = indexes.Select(x => x.InstanceId).ToList();

        return Page.Of(instanceIds, count);
    }

    public async ValueTask<Page<string>> FindManyIdsAsync<TOrderBy>(WorkflowInstanceFilter filter, PageArgs pageArgs, WorkflowInstanceOrder<TOrderBy> order, CancellationToken cancellationToken = default)
    {
        var query = QueryIndex(filter, order, pageArgs);
        var count = await query.CountAsync();
        var indexes = await query.ListAsync().ToList();
        var instanceIds = indexes.Select(x => x.InstanceId).ToList();

        return Page.Of(instanceIds, count);
    }
    
    public async ValueTask<Page<WorkflowInstanceSummary>> SummarizeManyAsync(WorkflowInstanceFilter filter, PageArgs pageArgs, CancellationToken cancellationToken = default)
    {
        var query = QueryIndex(filter, pageArgs);
        var count = await query.CountAsync();
        var indexes = await query.ListAsync().ToList();
        var summaries = MapSummaries(indexes).ToList();

        return Page.Of(summaries, count);
    }

    public async ValueTask<Page<WorkflowInstanceSummary>> SummarizeManyAsync<TOrderBy>(WorkflowInstanceFilter filter, PageArgs pageArgs, WorkflowInstanceOrder<TOrderBy> order, CancellationToken cancellationToken = default)
    {
        var query = QueryIndex(filter, order, pageArgs);
        var count = await query.CountAsync();
        var indexes = await query.ListAsync().ToList();
        var summaries = MapSummaries(indexes).ToList();

        return Page.Of(summaries, count);
    }

    public async ValueTask<IEnumerable<WorkflowInstanceSummary>> SummarizeManyAsync(WorkflowInstanceFilter filter, CancellationToken cancellationToken = default)
    {
        var query = QueryIndex(filter);
        var indexes = await query.ListAsync().ToList();
        return MapSummaries(indexes);
    }

    public async ValueTask<IEnumerable<WorkflowInstanceSummary>> SummarizeManyAsync<TOrder>(WorkflowInstanceFilter filter, WorkflowInstanceOrder<TOrder> order, CancellationToken cancellationToken = default)
    {
        var query = QueryIndex(filter, order);
        var indexes = await query.ListAsync().ToList();
        return MapSummaries(indexes);
    }

    public async ValueTask SaveAsync(WorkflowInstance instance, CancellationToken cancellationToken = default)
    {
        await session.SaveAsync(instance);
        await session.FlushAsync();
    }

    public async ValueTask AddAsync(WorkflowInstance instance, CancellationToken cancellationToken = default)
    {
        await session.SaveAsync(instance);
        await session.FlushAsync();
    }

    public async ValueTask UpdateAsync(WorkflowInstance instance, CancellationToken cancellationToken = default)
    {
        await session.SaveAsync(instance);
        await session.FlushAsync();
    }

    public async ValueTask SaveManyAsync(IEnumerable<WorkflowInstance> instances, CancellationToken cancellationToken = default)
    {
        foreach (var instance in instances) 
            await session.SaveAsync(instance);
        
        await session.FlushAsync();
    }

    public async ValueTask<long> DeleteAsync(WorkflowInstanceFilter filter, CancellationToken cancellationToken = default)
    {
        var pageArgs = PageArgs.FromRange(0, 100);
        var order = new WorkflowInstanceOrder<string>(x => x.Id, OrderDirection.Ascending);
        var count = 0;
        
        while (true)
        {
            var query = Query(filter, order, pageArgs);
            var records = await query.ListAsync().ToList();
            count += records.Count;
            
            if (records.Count == 0)
                break;

            foreach (var record in records) 
                session.Delete(record);
            
            pageArgs = pageArgs.Next();
        }
        
        return count;
    }

    private IQuery<WorkflowInstance, WorkflowInstanceIndex> Query(WorkflowInstanceFilter filter, PageArgs? pageArgs = null)
    {
        return Query<string>(filter, null, pageArgs);
    }

    private IQuery<WorkflowInstance, WorkflowInstanceIndex> Query<TOrderBy>(WorkflowInstanceFilter filter, WorkflowInstanceOrder<TOrderBy>? order = null, PageArgs? pageArgs = null)
    {
        var query = session.Query<WorkflowInstance, WorkflowInstanceIndex>().Apply(filter);
        if (order != null) query = query.Apply(order);

        if (pageArgs != null)
        {
            if (pageArgs.Offset != null) query.Skip(pageArgs.Offset.Value);
            if (pageArgs.Limit != null) query.Take(pageArgs.Limit.Value);
        }

        return query;
    }

    private IQueryIndex<WorkflowInstanceIndex> QueryIndex(WorkflowInstanceFilter filter, PageArgs? pageArgs = null)
    {
        return QueryIndex<string>(filter, null, pageArgs);
    }

    private IQueryIndex<WorkflowInstanceIndex> QueryIndex<TOrderBy>(WorkflowInstanceFilter filter, WorkflowInstanceOrder<TOrderBy>? order = null, PageArgs? pageArgs = null)
    {
        var query = session.QueryIndex<WorkflowInstanceIndex>().Apply(filter);
        if (order != null) query = query.Apply(order);

        if (pageArgs != null)
        {
            if (pageArgs.Offset != null) query.Skip(pageArgs.Offset.Value);
            if (pageArgs.Limit != null) query.Take(pageArgs.Limit.Value);
        }

        return query;
    }

    private IEnumerable<WorkflowInstanceSummary> MapSummaries(IEnumerable<WorkflowInstanceIndex> indexes)
    {
        return indexes.Select(MapSummary);
    }

    private WorkflowInstanceSummary MapSummary(WorkflowInstanceIndex index)
    {
        return new()
        {
            DefinitionId = index.DefinitionId,
            Name = index.Name,
            Version = index.Version,
            Status = index.Status,
            SubStatus = index.SubStatus,
            DefinitionVersionId = index.DefinitionVersionId,
            Id = index.InstanceId,
            CorrelationId = index.CorrelationId,
            CreatedAt = index.CreatedAt,
            IncidentCount = index.IncidentCount,
            FinishedAt = index.FinishedAt,
            UpdatedAt = index.UpdatedAt
        };
    }
}