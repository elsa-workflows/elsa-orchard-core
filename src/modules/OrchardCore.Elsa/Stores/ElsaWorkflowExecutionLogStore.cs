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

public class ElsaWorkflowExecutionLogStore(ISession session) : IWorkflowExecutionLogStore
{
    private const string Collection = ElsaCollections.WorkflowExecutionLogRecords;

    public async Task SaveManyAsync(IEnumerable<WorkflowExecutionLogRecord> records, CancellationToken cancellationToken = default)
    {
        foreach (var record in records)
        {
            var existingRecord = await Query(new() { Ids = new List<string> { record.Id } }).FirstOrDefaultAsync(cancellationToken);
            existingRecord = MapRecord(existingRecord, record);
            await session.SaveAsync(existingRecord, Collection);
        }

        await session.SaveChangesAsync(cancellationToken);
    }

    public async Task AddAsync(WorkflowExecutionLogRecord record, CancellationToken cancellationToken = default)
    {
        await session.SaveAsync(record, Collection);
        await session.SaveChangesAsync(cancellationToken);
    }

    public async Task AddManyAsync(IEnumerable<WorkflowExecutionLogRecord> records, CancellationToken cancellationToken = default)
    {
        foreach (var record in records)
            await session.SaveAsync(record, Collection);

        await session.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveAsync(WorkflowExecutionLogRecord record, CancellationToken cancellationToken = default)
    {
        var recordToSave = await Query(new() { Ids = new List<string> { record.Id } }).FirstOrDefaultAsync(cancellationToken);
        recordToSave = MapRecord(recordToSave, record);
        await session.SaveAsync(recordToSave, Collection);
        await session.SaveChangesAsync(cancellationToken);
    }

    public async Task<WorkflowExecutionLogRecord?> FindAsync(WorkflowExecutionLogRecordFilter filter, CancellationToken cancellationToken = default)
    {
        return await Query(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<WorkflowExecutionLogRecord?> FindAsync<TOrderBy>(WorkflowExecutionLogRecordFilter filter, WorkflowExecutionLogRecordOrder<TOrderBy> order, CancellationToken cancellationToken = default)
    {
        return await Query(filter, order).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Page<WorkflowExecutionLogRecord>> FindManyAsync(WorkflowExecutionLogRecordFilter filter, PageArgs pageArgs, CancellationToken cancellationToken = default)
    {
        var query = Query(filter, pageArgs);
        var count = await query.CountAsync(cancellationToken);
        var records = await query.ListAsync(cancellationToken).ToList();

        return Page.Of(records, count);
    }

    public async Task<Page<WorkflowExecutionLogRecord>> FindManyAsync<TOrderBy>(WorkflowExecutionLogRecordFilter filter, PageArgs pageArgs, WorkflowExecutionLogRecordOrder<TOrderBy> order, CancellationToken cancellationToken = default)
    {
        var query = Query(filter, order, pageArgs);
        var count = await query.CountAsync(cancellationToken);
        var records = await query.ListAsync(cancellationToken).ToList();

        return Page.Of(records, count);
    }

    public async Task<long> DeleteManyAsync(WorkflowExecutionLogRecordFilter filter, CancellationToken cancellationToken = default)
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

    private IQuery<WorkflowExecutionLogRecord, WorkflowExecutionLogRecordIndex> Query(WorkflowExecutionLogRecordFilter filter, PageArgs? pageArgs = null)
    {
        return Query<string>(filter, pageArgs: pageArgs);
    }

    private IQuery<WorkflowExecutionLogRecord, WorkflowExecutionLogRecordIndex> Query<TOrderBy>(WorkflowExecutionLogRecordFilter filter, WorkflowExecutionLogRecordOrder<TOrderBy>? order = null, PageArgs? pageArgs = null)
    {
        var query = session.Query<WorkflowExecutionLogRecord, WorkflowExecutionLogRecordIndex>(Collection).Apply(filter);
        if (order != null) query = query.Apply(order);

        if (pageArgs != null)
        {
            if (pageArgs.Offset != null) query.Skip(pageArgs.Offset.Value);
            if (pageArgs.Limit != null) query.Take(pageArgs.Limit.Value);
        }

        return query;
    }
    
    private WorkflowExecutionLogRecord MapRecord(WorkflowExecutionLogRecord? target, WorkflowExecutionLogRecord source)
    {
        if (target == null)
            return source;

        target.Id = source.Id;
        target.WorkflowInstanceId = source.WorkflowInstanceId;
        target.Timestamp = source.Timestamp;
        target.EventName = source.EventName;
        target.ActivityId = source.ActivityId;
        target.ActivityType = source.ActivityType;
        target.ActivityNodeId = source.ActivityNodeId;
        target.ActivityTypeVersion = source.ActivityTypeVersion;
        target.TenantId = source.TenantId;
        target.Payload = source.Payload;
        target.ActivityInstanceId = source.ActivityInstanceId;
        target.Message = source.Message;
        target.ParentActivityInstanceId = source.ParentActivityInstanceId;
        target.Sequence = source.Sequence;
        target.Source = source.Source;
        target.WorkflowDefinitionId = source.WorkflowDefinitionId;
        target.WorkflowDefinitionVersionId = source.WorkflowDefinitionVersionId;
        target.WorkflowVersion = source.WorkflowVersion;

        return target;
    }
}