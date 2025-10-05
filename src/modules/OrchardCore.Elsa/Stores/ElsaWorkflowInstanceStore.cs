using Elsa.Common.Entities;
using Elsa.Common.Models;
using Elsa.Workflows.Management;
using Elsa.Workflows.Management.Entities;
using Elsa.Workflows.Management.Filters;
using Elsa.Workflows.Management.Models;
using Open.Linq.AsyncExtensions;
using OrchardCore.Elsa.Indexes;
using OrchardCore.Elsa.Extensions;
using YesSql;

namespace OrchardCore.Elsa.Stores;

public class ElsaWorkflowInstanceStore(ISession session) : IWorkflowInstanceStore
{
    private const string Collection = ElsaCollections.WorkflowInstances;

    public async ValueTask<WorkflowInstance?> FindAsync(WorkflowInstanceFilter filter, CancellationToken cancellationToken = default)
    {
        var record = await Query(filter).FirstOrDefaultAsync(cancellationToken);
        var recordId = record?.Id;
        return record;
    }

    public async ValueTask<Page<WorkflowInstance>> FindManyAsync(WorkflowInstanceFilter filter, PageArgs pageArgs, CancellationToken cancellationToken = default)
    {
        var query = Query(filter, pageArgs);
        var count = await query.CountAsync(cancellationToken);
        var records = await query.ListAsync(cancellationToken).ToList();

        return Page.Of(records, count);
    }

    public async ValueTask<Page<WorkflowInstance>> FindManyAsync<TOrderBy>(WorkflowInstanceFilter filter, PageArgs pageArgs, WorkflowInstanceOrder<TOrderBy> order, CancellationToken cancellationToken = default)
    {
        var query = Query(filter, order, pageArgs);
        var count = await query.CountAsync(cancellationToken);
        var records = await query.ListAsync(cancellationToken).ToList();

        return Page.Of(records, count);
    }

    public async ValueTask<IEnumerable<WorkflowInstance>> FindManyAsync(WorkflowInstanceFilter filter, CancellationToken cancellationToken = default)
    {
        var records = await Query(filter).ListAsync(cancellationToken);
        return records;
    }

    public async ValueTask<IEnumerable<WorkflowInstance>> FindManyAsync<TOrderBy>(WorkflowInstanceFilter filter, WorkflowInstanceOrder<TOrderBy> order, CancellationToken cancellationToken = default)
    {
        var records = await Query(filter, order).ListAsync(cancellationToken);
        return records;
    }

    public async ValueTask<long> CountAsync(WorkflowInstanceFilter filter, CancellationToken cancellationToken = default)
    {
        return await Query(filter).CountAsync(cancellationToken);
    }

    public async ValueTask<IEnumerable<string>> FindManyIdsAsync(WorkflowInstanceFilter filter, CancellationToken cancellationToken = default)
    {
        var query = QueryIndex(filter);
        var indexes = await query.ListAsync(cancellationToken).ToList();
        return indexes.Select(x => x.InstanceId);
    }

    public async ValueTask<Page<string>> FindManyIdsAsync(WorkflowInstanceFilter filter, PageArgs pageArgs, CancellationToken cancellationToken = default)
    {
        var query = QueryIndex(filter, pageArgs);
        var count = await query.CountAsync(cancellationToken);
        var indexes = await query.ListAsync(cancellationToken).ToList();
        var instanceIds = indexes.Select(x => x.InstanceId).ToList();

        return Page.Of(instanceIds, count);
    }

    public async ValueTask<Page<string>> FindManyIdsAsync<TOrderBy>(WorkflowInstanceFilter filter, PageArgs pageArgs, WorkflowInstanceOrder<TOrderBy> order, CancellationToken cancellationToken = default)
    {
        var query = QueryIndex(filter, order, pageArgs);
        var count = await query.CountAsync(cancellationToken);
        var indexes = await query.ListAsync(cancellationToken).ToList();
        var instanceIds = indexes.Select(x => x.InstanceId).ToList();

        return Page.Of(instanceIds, count);
    }

    public async ValueTask<Page<WorkflowInstanceSummary>> SummarizeManyAsync(WorkflowInstanceFilter filter, PageArgs pageArgs, CancellationToken cancellationToken = default)
    {
        var query = QueryIndex(filter, pageArgs);
        var count = await query.CountAsync(cancellationToken);
        var indexes = await query.ListAsync(cancellationToken).ToList();
        var summaries = MapSummaries(indexes).ToList();

        return Page.Of(summaries, count);
    }

    public async ValueTask<Page<WorkflowInstanceSummary>> SummarizeManyAsync<TOrderBy>(WorkflowInstanceFilter filter, PageArgs pageArgs, WorkflowInstanceOrder<TOrderBy> order, CancellationToken cancellationToken = default)
    {
        var query = QueryIndex(filter, order, pageArgs);
        var count = await query.CountAsync(cancellationToken);
        var indexes = await query.ListAsync(cancellationToken).ToList();
        var summaries = MapSummaries(indexes).ToList();

        return Page.Of(summaries, count);
    }

    public async ValueTask<IEnumerable<WorkflowInstanceSummary>> SummarizeManyAsync(WorkflowInstanceFilter filter, CancellationToken cancellationToken = default)
    {
        var query = QueryIndex(filter);
        var indexes = await query.ListAsync(cancellationToken).ToList();
        return MapSummaries(indexes);
    }

    public async ValueTask<IEnumerable<WorkflowInstanceSummary>> SummarizeManyAsync<TOrder>(WorkflowInstanceFilter filter, WorkflowInstanceOrder<TOrder> order, CancellationToken cancellationToken = default)
    {
        var query = QueryIndex(filter, order);
        var indexes = await query.ListAsync(cancellationToken).ToList();
        return MapSummaries(indexes);
    }

    public async ValueTask SaveAsync(WorkflowInstance instance, CancellationToken cancellationToken = default)
    {
        var record = await session.Query<WorkflowInstance, WorkflowInstanceIndex>(Collection).Where(x => x.InstanceId == instance.Id).FirstOrDefaultAsync(cancellationToken);

        if (record != null)
        {
            record.Status = instance.Status;
            record.SubStatus = instance.SubStatus;
            record.CorrelationId = instance.CorrelationId;
            record.DefinitionId = instance.DefinitionId;
            record.DefinitionVersionId = instance.DefinitionVersionId;
            record.IncidentCount = instance.IncidentCount;
            record.CreatedAt = instance.CreatedAt;
            record.FinishedAt = instance.FinishedAt;
            record.UpdatedAt = instance.UpdatedAt;
            record.IsSystem = instance.IsSystem;
            record.Name = instance.Name;
            record.ParentWorkflowInstanceId = instance.ParentWorkflowInstanceId;
            record.Version = instance.Version;
            record.WorkflowState = instance.WorkflowState;
            record.TenantId = instance.TenantId;
        }
        else
        {
            record = instance;
        }
        
        await session.SaveAsync(record, Collection);
        await session.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask AddAsync(WorkflowInstance instance, CancellationToken cancellationToken = default)
    {
        await session.SaveAsync(instance, Collection);
        await session.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask UpdateAsync(WorkflowInstance instance, CancellationToken cancellationToken = default)
    {
        await session.SaveAsync(instance, Collection);
        await session.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask SaveManyAsync(IEnumerable<WorkflowInstance> instances, CancellationToken cancellationToken = default)
    {
        foreach (var instance in instances)
            await session.SaveAsync(instance, Collection);

        await session.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask<long> DeleteAsync(WorkflowInstanceFilter filter, CancellationToken cancellationToken = default)
    {
        var pageArgs = PageArgs.FromRange(0, 100);
        var order = new WorkflowInstanceOrder<string>(x => x.Id, OrderDirection.Ascending);
        var count = 0;

        while (true)
        {
            var query = Query(filter, order, pageArgs);
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

    public async Task UpdateUpdatedTimestampAsync(string workflowInstanceId, DateTimeOffset value, CancellationToken cancellationToken = default)
    {
        var instance = await session.Query<WorkflowInstance, WorkflowInstanceIndex>(Collection).Where(x => x.InstanceId == workflowInstanceId).FirstOrDefaultAsync(cancellationToken);
        if (instance == null)
            return;
        instance.UpdatedAt = value;
        await session.SaveAsync(instance, Collection);
        await session.SaveChangesAsync(cancellationToken);
    }

    private IQuery<WorkflowInstance, WorkflowInstanceIndex> Query(WorkflowInstanceFilter filter, PageArgs? pageArgs = null)
    {
        return Query<string>(filter, null, pageArgs);
    }

    private IQuery<WorkflowInstance, WorkflowInstanceIndex> Query<TOrderBy>(WorkflowInstanceFilter filter, WorkflowInstanceOrder<TOrderBy>? order = null, PageArgs? pageArgs = null)
    {
        var query = session.Query<WorkflowInstance, WorkflowInstanceIndex>(Collection).Apply(filter);
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
        var query = session.QueryIndex<WorkflowInstanceIndex>(Collection).Apply(filter);
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