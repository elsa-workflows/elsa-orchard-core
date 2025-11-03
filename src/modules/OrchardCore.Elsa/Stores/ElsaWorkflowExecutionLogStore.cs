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

public class ElsaWorkflowExecutionLogStore(ISession session, IPayloadSerializer payloadSerializer) : IWorkflowExecutionLogStore
{
    private const string Collection = ElsaCollections.WorkflowExecutionLogRecords;

    public async Task SaveManyAsync(IEnumerable<WorkflowExecutionLogRecord> records, CancellationToken cancellationToken = default)
    {
        foreach (var record in records)
        {
            var document = await Query(new() { Ids = new List<string> { record.Id } }).FirstOrDefaultAsync(cancellationToken);
            document = Map(document, record);
            await session.SaveAsync(document, Collection);
        }

        await session.FlushAsync(cancellationToken);
    }

    public async Task AddAsync(WorkflowExecutionLogRecord record, CancellationToken cancellationToken = default)
    {
        var document = Map(null, record);
        await session.SaveAsync(document, Collection);
        await session.FlushAsync(cancellationToken);
    }

    public async Task AddManyAsync(IEnumerable<WorkflowExecutionLogRecord> records, CancellationToken cancellationToken = default)
    {
        foreach (var record in records)
        {
            var document = Map(null, record);
            await session.SaveAsync(document, Collection);
        }

        await session.FlushAsync(cancellationToken);
    }

    public async Task SaveAsync(WorkflowExecutionLogRecord record, CancellationToken cancellationToken = default)
    {
        var document = await Query(new() { Ids = new List<string> { record.Id } }).FirstOrDefaultAsync(cancellationToken);
        document = Map(document, record);
        await session.SaveAsync(document, Collection);
        await session.FlushAsync(cancellationToken);
    }

    public async Task<WorkflowExecutionLogRecord?> FindAsync(WorkflowExecutionLogRecordFilter filter, CancellationToken cancellationToken = default)
    {
        var document = await Query(filter).FirstOrDefaultAsync(cancellationToken);
        return Map(document);
    }

    public async Task<WorkflowExecutionLogRecord?> FindAsync<TOrderBy>(WorkflowExecutionLogRecordFilter filter, WorkflowExecutionLogRecordOrder<TOrderBy> order, CancellationToken cancellationToken = default)
    {
        var document = await Query(filter, order).FirstOrDefaultAsync(cancellationToken);
        return Map(document);
    }

    public async Task<Page<WorkflowExecutionLogRecord>> FindManyAsync(WorkflowExecutionLogRecordFilter filter, PageArgs pageArgs, CancellationToken cancellationToken = default)
    {
        var query = Query(filter, pageArgs);
        var count = await query.CountAsync(cancellationToken);
        var documents = await query.ListAsync(cancellationToken).ToList();

        return Page.Of(Map(documents).ToList(), count);
    }

    public async Task<Page<WorkflowExecutionLogRecord>> FindManyAsync<TOrderBy>(WorkflowExecutionLogRecordFilter filter, PageArgs pageArgs, WorkflowExecutionLogRecordOrder<TOrderBy> order, CancellationToken cancellationToken = default)
    {
        var query = Query(filter, order, pageArgs);
        var count = await query.CountAsync(cancellationToken);
        var documents = await query.ListAsync(cancellationToken).ToList();

        return Page.Of(Map(documents).ToList(), count);
    }

    public async Task<long> DeleteManyAsync(WorkflowExecutionLogRecordFilter filter, CancellationToken cancellationToken = default)
    {
        var pageArgs = PageArgs.FromRange(0, 100);
        var count = 0;

        while (true)
        {
            var query = Query(filter).OrderBy(x => x.RecordId).Skip(pageArgs.Offset!.Value).Take(pageArgs.Limit!.Value);
            var documents = await query.ListAsync(cancellationToken).ToList();
            count += documents.Count;

            if (documents.Count == 0)
                break;

            foreach (var document in documents)
                session.Delete(document, Collection);

            pageArgs = pageArgs.Next();
        }

        return count;
    }

    private IQuery<WorkflowExecutionLogRecordDocument, WorkflowExecutionLogRecordIndex> Query(WorkflowExecutionLogRecordFilter filter, PageArgs? pageArgs = null)
    {
        return Query<string>(filter, pageArgs: pageArgs);
    }

    private IQuery<WorkflowExecutionLogRecordDocument, WorkflowExecutionLogRecordIndex> Query<TOrderBy>(WorkflowExecutionLogRecordFilter filter, WorkflowExecutionLogRecordOrder<TOrderBy>? order = null, PageArgs? pageArgs = null)
    {
        var query = session.Query<WorkflowExecutionLogRecordDocument, WorkflowExecutionLogRecordIndex>(Collection).Apply(filter);
        if (order != null) query = query.Apply(order);

        if (pageArgs != null)
        {
            if (pageArgs.Offset != null) query.Skip(pageArgs.Offset.Value);
            if (pageArgs.Limit != null) query.Take(pageArgs.Limit.Value);
        }

        return query;
    }

    private WorkflowExecutionLogRecordDocument Map(WorkflowExecutionLogRecordDocument? target, WorkflowExecutionLogRecord source)
    {
        if (target == null)
            target = new();

        target.RecordId = source.Id;
        target.TenantId = source.TenantId;
        target.WorkflowInstanceId = source.WorkflowInstanceId;
        target.WorkflowDefinitionId = source.WorkflowDefinitionId;
        target.WorkflowDefinitionVersionId = source.WorkflowDefinitionVersionId;
        target.WorkflowVersion = source.WorkflowVersion;
        target.ActivityId = source.ActivityId;
        target.ActivityNodeId = source.ActivityNodeId;
        target.ActivityType = source.ActivityType;
        target.ActivityTypeVersion = source.ActivityTypeVersion;
        target.ActivityInstanceId = source.ActivityInstanceId;
        target.ParentActivityInstanceId = source.ParentActivityInstanceId;
        target.EventName = source.EventName;
        target.Message = source.Message;
        target.Source = source.Source;
        target.Timestamp = source.Timestamp;
        target.Sequence = source.Sequence;

        if (source.Payload != null)
            target.SerializedPayload = payloadSerializer.Serialize(source.Payload);

        return target;
    }

    private IEnumerable<WorkflowExecutionLogRecord> Map(IEnumerable<WorkflowExecutionLogRecordDocument> source)
    {
        return source.Select(x => Map(x)!);
    }

    private WorkflowExecutionLogRecord? Map(WorkflowExecutionLogRecordDocument? source)
    {
        if (source == null)
            return null;

        var payload = source.SerializedPayload != null
            ? payloadSerializer.Deserialize(source.SerializedPayload)
            : null;

        return new()
        {
            Id = source.RecordId,
            TenantId = source.TenantId,
            WorkflowInstanceId = source.WorkflowInstanceId,
            WorkflowDefinitionId = source.WorkflowDefinitionId,
            WorkflowDefinitionVersionId = source.WorkflowDefinitionVersionId,
            WorkflowVersion = source.WorkflowVersion,
            ActivityId = source.ActivityId,
            ActivityNodeId = source.ActivityNodeId,
            ActivityType = source.ActivityType,
            ActivityTypeVersion = source.ActivityTypeVersion,
            ActivityInstanceId = source.ActivityInstanceId,
            ParentActivityInstanceId = source.ParentActivityInstanceId,
            EventName = source.EventName,
            Message = source.Message,
            Source = source.Source,
            Timestamp = source.Timestamp,
            Sequence = source.Sequence,
            Payload = payload
        };
    }
}
