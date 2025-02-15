using System.Threading.Tasks;
using JetBrains.Annotations;
using OrchardCore.Data.Migration;
using OrchardCore.ElsaWorkflows.Indexes;
using YesSql.Sql;

namespace OrchardCore.ElsaWorkflows.Migrations;

[UsedImplicitly]
public class StoredTriggerMigrations : DataMigration
{
    private const string Collection = ElsaCollections.StoredTriggers;
    
    [UsedImplicitly]
    public async Task<int> CreateAsync()
    {
        await SchemaBuilder.CreateMapIndexTableAsync<StoredTriggerIndex>(table => table
            .Column<string>(nameof(StoredTriggerIndex.TriggerId), c => c.NotNull())
            .Column<string>(nameof(StoredTriggerIndex.WorkflowDefinitionId), c => c.NotNull())
            .Column<string>(nameof(StoredTriggerIndex.WorkflowDefinitionVersionId), c => c.NotNull())
            .Column<string>(nameof(StoredTriggerIndex.Name), c => c.NotNull())
            .Column<string>(nameof(StoredTriggerIndex.ActivityId), c => c.NotNull())
            .Column<string>(nameof(StoredTriggerIndex.Hash), c => c.Nullable())
        , Collection);
        
        await SchemaBuilder.AlterIndexTableAsync<StoredTriggerIndex>(table =>
        {
            table.CreateIndex("IDX_StoredTriggerIndex_Name", nameof(StoredTriggerIndex.Name));
            table.CreateIndex("IDX_StoredTriggerIndex_WorkflowDefinitionId", nameof(StoredTriggerIndex.WorkflowDefinitionId));
            table.CreateIndex("IDX_StoredTriggerIndex_WorkflowDefinitionVersionId", nameof(StoredTriggerIndex.WorkflowDefinitionVersionId));
            table.CreateIndex("IDX_StoredTriggerIndex_ActivityId", nameof(StoredTriggerIndex.ActivityId));
            table.CreateIndex("IDX_StoredTriggerIndex_Hash", nameof(StoredTriggerIndex.Hash));
        }, Collection);
        
        return 1;
    }
}