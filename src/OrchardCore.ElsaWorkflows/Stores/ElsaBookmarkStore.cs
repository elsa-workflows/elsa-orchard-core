using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Common.Models;
using Elsa.Workflows.Runtime;
using Elsa.Workflows.Runtime.Entities;
using Elsa.Workflows.Runtime.Filters;
using Open.Linq.AsyncExtensions;
using OrchardCore.ElsaWorkflows.Extensions;
using OrchardCore.ElsaWorkflows.Indexes;
using YesSql;

namespace OrchardCore.ElsaWorkflows.Stores;

public class ElsaBookmarkStore(ISession session) : IBookmarkStore
{
    private const string Collection = ElsaCollections.StoredBookmarks;
    
    public async ValueTask SaveAsync(StoredBookmark record, CancellationToken cancellationToken = default)
    {
        await session.SaveAsync(record, Collection);
        await session.SaveChangesAsync();
    }

    public async ValueTask SaveManyAsync(IEnumerable<StoredBookmark> records, CancellationToken cancellationToken)
    {
        foreach (var record in records) 
            await session.SaveAsync(record, Collection);
        
        await session.SaveChangesAsync();
    }

    public async ValueTask<StoredBookmark?> FindAsync(BookmarkFilter filter, CancellationToken cancellationToken = default)
    {
        return await Query(filter).FirstOrDefaultAsync();
    }

    public async ValueTask<IEnumerable<StoredBookmark>> FindManyAsync(BookmarkFilter filter, CancellationToken cancellationToken = default)
    {
        return await Query(filter).ListAsync();
    }

    public async ValueTask<long> DeleteAsync(BookmarkFilter filter, CancellationToken cancellationToken = default)
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
        
        await session.SaveChangesAsync();
        return count;
    }
    
    private IQuery<StoredBookmark, StoredBookmarkIndex> Query(BookmarkFilter filter)
    {
        return session.Query<StoredBookmark, StoredBookmarkIndex>(Collection).Apply(filter);
    }
}