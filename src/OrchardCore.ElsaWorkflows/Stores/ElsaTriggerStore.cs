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

public class ElsaTriggerStore(ISession session) : ITriggerStore
{
    private const string Collection = ElsaCollections.StoredTriggers;
    
    public async ValueTask SaveAsync(StoredTrigger record, CancellationToken cancellationToken = default)
    {
        await session.SaveAsync(record, Collection);
        await session.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask SaveManyAsync(IEnumerable<StoredTrigger> records, CancellationToken cancellationToken = default)
    {
        foreach (var record in records) 
            await session.SaveAsync(record, Collection);
        
        await session.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask<StoredTrigger?> FindAsync(TriggerFilter filter, CancellationToken cancellationToken = default)
    {
        return await Query(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<IEnumerable<StoredTrigger>> FindManyAsync(TriggerFilter filter, CancellationToken cancellationToken = default)
    {
        return await Query(filter).ListAsync(cancellationToken);
    }

    public ValueTask<Page<StoredTrigger>> FindManyAsync(TriggerFilter filter, PageArgs pageArgs, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<Page<StoredTrigger>> FindManyAsync<TProp>(TriggerFilter filter, PageArgs pageArgs, StoredTriggerOrder<TProp> order, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async ValueTask ReplaceAsync(IEnumerable<StoredTrigger> removed, IEnumerable<StoredTrigger> added, CancellationToken cancellationToken = default)
    {
        var removedList = removed.ToList();
        
        if(removedList.Count > 0)
        {
            var filter = new TriggerFilter { Ids = removedList.Select(r => r.Id).ToList() };
            await DeleteManyAsync(filter, cancellationToken);
        }
        
        await SaveManyAsync(added, cancellationToken);
    }

    public async ValueTask<long> DeleteManyAsync(TriggerFilter filter, CancellationToken cancellationToken = default)
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
    
    private IQuery<StoredTrigger, StoredTriggerIndex> Query(TriggerFilter filter)
    {
        return session.Query<StoredTrigger, StoredTriggerIndex>(Collection).Apply(filter);
    }
}