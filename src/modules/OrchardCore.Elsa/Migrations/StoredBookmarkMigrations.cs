using JetBrains.Annotations;
using OrchardCore.Data.Migration;
using OrchardCore.Elsa.Indexes;
using YesSql.Sql;

namespace OrchardCore.Elsa.Migrations;

[UsedImplicitly]
public class StoredBookmarkMigrations : DataMigration
{
    private const string Collection = ElsaCollections.StoredBookmarks;
    
    [UsedImplicitly]
    public async Task<int> CreateAsync()
    {
        await SchemaBuilder.CreateMapIndexTableAsync<StoredBookmarkIndex>(table => table
            .Column<string>(nameof(StoredBookmarkIndex.BookmarkId), c => c.NotNull())
            .Column<string>(nameof(StoredBookmarkIndex.Name), c => c.NotNull())
            .Column<string>(nameof(StoredBookmarkIndex.WorkflowInstanceId), c => c.NotNull())
            .Column<string>(nameof(StoredBookmarkIndex.Hash), c => c.NotNull())
            .Column<string>(nameof(StoredBookmarkIndex.CorrelationId), c => c.Nullable())
            .Column<string>(nameof(StoredBookmarkIndex.ActivityInstanceId), c => c.Nullable())
        , Collection);
        
        await SchemaBuilder.AlterIndexTableAsync<StoredBookmarkIndex>(table =>
        {
            table.CreateIndex("IDX_StoredBookmarkIndex_BookmarkId", nameof(StoredBookmarkIndex.BookmarkId));
            table.CreateIndex("IDX_StoredBookmarkIndex_Name", nameof(StoredBookmarkIndex.Name));
            table.CreateIndex("IDX_StoredBookmarkIndex_WorkflowInstanceId", nameof(StoredBookmarkIndex.WorkflowInstanceId));
            table.CreateIndex("IDX_StoredBookmarkIndex_Hash", nameof(StoredBookmarkIndex.Hash));
            table.CreateIndex("IDX_StoredBookmarkIndex_CorrelationId", nameof(StoredBookmarkIndex.CorrelationId));
            table.CreateIndex("IDX_StoredBookmarkIndex_ActivityInstanceId", nameof(StoredBookmarkIndex.ActivityInstanceId));
        }, Collection);
        
        return 1;
    }
}