using Elsa.Common.Entities;
using Elsa.Common.Models;
using Elsa.Workflows;
using Elsa.Workflows.Management;
using Elsa.Workflows.Management.Entities;
using Elsa.Workflows.Management.Filters;
using Elsa.Workflows.Management.Models;
using Open.Linq.AsyncExtensions;
using OrchardCore.Elsa.Documents;
using OrchardCore.Elsa.Indexes;
using OrchardCore.Elsa.Extensions;
using YesSql;

namespace OrchardCore.Elsa.Stores;

public class ElsaWorkflowInstanceStore(ISession session, IWorkflowStateSerializer workflowStateSerializer) : IWorkflowInstanceStore
{
    private const string Collection = ElsaCollections.WorkflowInstances;

    public async ValueTask<WorkflowInstance?> FindAsync(WorkflowInstanceFilter filter, CancellationToken cancellationToken = default)
    {
        var document = await Query(filter).FirstOrDefaultAsync(cancellationToken);
        return Map(document);
    }

    public async ValueTask<Page<WorkflowInstance>> FindManyAsync(WorkflowInstanceFilter filter, PageArgs pageArgs, CancellationToken cancellationToken = default)
    {
        var query = Query(filter, pageArgs);
        var count = await query.CountAsync(cancellationToken);
        var documents = await query.ListAsync(cancellationToken).ToList();

        return Page.Of(Map(documents).ToList(), count);
    }

    public async ValueTask<Page<WorkflowInstance>> FindManyAsync<TOrderBy>(WorkflowInstanceFilter filter, PageArgs pageArgs, WorkflowInstanceOrder<TOrderBy> order, CancellationToken cancellationToken = default)
    {
        var query = Query(filter, order, pageArgs);
        var count = await query.CountAsync(cancellationToken);
        var documents = await query.ListAsync(cancellationToken).ToList();

        return Page.Of(Map(documents).ToList(), count);
    }

    public async ValueTask<IEnumerable<WorkflowInstance>> FindManyAsync(WorkflowInstanceFilter filter, CancellationToken cancellationToken = default)
    {
        var documents = await Query(filter).ListAsync(cancellationToken);
        return Map(documents);
    }

    public async ValueTask<IEnumerable<WorkflowInstance>> FindManyAsync<TOrderBy>(WorkflowInstanceFilter filter, WorkflowInstanceOrder<TOrderBy> order, CancellationToken cancellationToken = default)
    {
        var documents = await Query(filter, order).ListAsync(cancellationToken);
        return Map(documents);
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
        var document = await session.Query<WorkflowInstanceDocument, WorkflowInstanceIndex>(Collection).Where(x => x.InstanceId == instance.Id).FirstOrDefaultAsync(cancellationToken);
        document = Map(document, instance);
        await session.SaveAsync(document, Collection);
        await session.FlushAsync(cancellationToken);
    }

    public async ValueTask AddAsync(WorkflowInstance instance, CancellationToken cancellationToken = default)
    {
        var document = Map(null, instance);
        await session.SaveAsync(document, Collection);
        await session.FlushAsync(cancellationToken);
    }

    public async ValueTask UpdateAsync(WorkflowInstance instance, CancellationToken cancellationToken = default)
    {
        var document = await session.Query<WorkflowInstanceDocument, WorkflowInstanceIndex>(Collection).Where(x => x.InstanceId == instance.Id).FirstOrDefaultAsync(cancellationToken);
        document = Map(document, instance);
        await session.SaveAsync(document, Collection);
        await session.FlushAsync(cancellationToken);
    }

    public async ValueTask SaveManyAsync(IEnumerable<WorkflowInstance> instances, CancellationToken cancellationToken = default)
    {
        foreach (var instance in instances)
        {
            var document = await session.Query<WorkflowInstanceDocument, WorkflowInstanceIndex>(Collection).Where(x => x.InstanceId == instance.Id).FirstOrDefaultAsync(cancellationToken);
            document = Map(document, instance);
            await session.SaveAsync(document, Collection);
        }

        await session.FlushAsync(cancellationToken);
    }

    public async ValueTask<long> DeleteAsync(WorkflowInstanceFilter filter, CancellationToken cancellationToken = default)
    {
        var pageArgs = PageArgs.FromRange(0, 100);
        var order = new WorkflowInstanceOrder<string>(x => x.Id, OrderDirection.Ascending);
        var count = 0;

        while (true)
        {
            var query = Query(filter, order, pageArgs);
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

    public async Task UpdateUpdatedTimestampAsync(string workflowInstanceId, DateTimeOffset value, CancellationToken cancellationToken = default)
    {
        var document = await session.Query<WorkflowInstanceDocument, WorkflowInstanceIndex>(Collection).Where(x => x.InstanceId == workflowInstanceId).FirstOrDefaultAsync(cancellationToken);
        if (document == null)
            return;
        document.UpdatedAt = value;
        await session.SaveAsync(document, Collection);
        await session.SaveChangesAsync(cancellationToken);
    }

    private IQuery<WorkflowInstanceDocument, WorkflowInstanceIndex> Query(WorkflowInstanceFilter filter, PageArgs? pageArgs = null)
    {
        return Query<string>(filter, null, pageArgs);
    }

    private IQuery<WorkflowInstanceDocument, WorkflowInstanceIndex> Query<TOrderBy>(WorkflowInstanceFilter filter, WorkflowInstanceOrder<TOrderBy>? order = null, PageArgs? pageArgs = null)
    {
        var query = session.Query<WorkflowInstanceDocument, WorkflowInstanceIndex>(Collection).Apply(filter);
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

    private WorkflowInstanceDocument Map(WorkflowInstanceDocument? target, WorkflowInstance source)
    {
        if (target == null)
            target = new();

        target.InstanceId = source.Id;
        target.TenantId = source.TenantId;
        target.DefinitionId = source.DefinitionId;
        target.DefinitionVersionId = source.DefinitionVersionId;
        target.Version = source.Version;
        target.CorrelationId = source.CorrelationId;
        target.Name = source.Name;
        target.ParentWorkflowInstanceId = source.ParentWorkflowInstanceId;
        target.Status = source.Status;
        target.SubStatus = source.SubStatus;
        target.IncidentCount = source.IncidentCount;
        target.IsSystem = source.IsSystem;
        target.CreatedAt = source.CreatedAt;
        target.FinishedAt = source.FinishedAt;
        target.UpdatedAt = source.UpdatedAt;

        var workflowState = source.WorkflowState;
        var json = workflowStateSerializer.Serialize(workflowState);
        target.SerializedWorkflowState = json;
        
        return target;
    }

    private IEnumerable<WorkflowInstance> Map(IEnumerable<WorkflowInstanceDocument> source)
    {
        return source.Select(x => Map(x)!);
    }

    private WorkflowInstance? Map(WorkflowInstanceDocument? source)
    {
        if (source == null)
            return null;

        var workflowState = source.SerializedWorkflowState != null
            ? workflowStateSerializer.Deserialize(source.SerializedWorkflowState)
            : null;

        return new()
        {
            Id = source.InstanceId,
            TenantId = source.TenantId,
            DefinitionId = source.DefinitionId,
            DefinitionVersionId = source.DefinitionVersionId,
            Version = source.Version,
            CorrelationId = source.CorrelationId,
            Name = source.Name,
            ParentWorkflowInstanceId = source.ParentWorkflowInstanceId,
            Status = source.Status,
            SubStatus = source.SubStatus,
            IncidentCount = source.IncidentCount,
            IsSystem = source.IsSystem,
            CreatedAt = source.CreatedAt,
            FinishedAt = source.FinishedAt,
            UpdatedAt = source.UpdatedAt,
            WorkflowState = workflowState!
        };
    }
}