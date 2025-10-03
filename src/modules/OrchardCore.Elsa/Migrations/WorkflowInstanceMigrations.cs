using Elsa.Workflows;
using JetBrains.Annotations;
using OrchardCore.Data.Migration;
using OrchardCore.Elsa.Indexes;
using YesSql.Sql;

namespace OrchardCore.Elsa.Migrations;

[UsedImplicitly]
public class WorkflowInstanceMigrations : DataMigration
{
    private const string Collection = ElsaCollections.WorkflowInstances;

    [UsedImplicitly]
    public async Task<int> CreateAsync()
    {
        await SchemaBuilder.CreateMapIndexTableAsync<WorkflowInstanceIndex>(table => table
                .Column<string>(nameof(WorkflowInstanceIndex.InstanceId), c => c.NotNull().WithLength(32))
                .Column<string>(nameof(WorkflowInstanceIndex.DefinitionId), c => c.NotNull().WithLength(32))
                .Column<string>(nameof(WorkflowInstanceIndex.DefinitionVersionId), c => c.NotNull().WithLength(32))
                .Column<int>(nameof(WorkflowInstanceIndex.Version))
                .Column<string>(nameof(WorkflowInstanceIndex.Name), c => c.Nullable())
                .Column<string>(nameof(WorkflowInstanceIndex.CorrelationId), c => c.Nullable())
                .Column<string>(nameof(WorkflowInstanceIndex.ParentInstanceId), c => c.Nullable())
                .Column<WorkflowStatus>(nameof(WorkflowInstanceIndex.Status), c => c.NotNull())
                .Column<WorkflowSubStatus>(nameof(WorkflowInstanceIndex.SubStatus), c => c.NotNull())
                .Column<bool>(nameof(WorkflowInstanceIndex.HasIncidents), c => c.NotNull())
                .Column<int>(nameof(WorkflowInstanceIndex.IncidentCount), c => c.NotNull())
                .Column<bool>(nameof(WorkflowInstanceIndex.IsSystem), c => c.NotNull())
                .Column<DateTimeOffset>(nameof(WorkflowInstanceIndex.CreatedAt), c => c.NotNull())
                .Column<DateTimeOffset>(nameof(WorkflowInstanceIndex.UpdatedAt), c => c.Nullable())
                .Column<DateTimeOffset>(nameof(WorkflowInstanceIndex.FinishedAt), c => c.Nullable())
            , Collection);

        await SchemaBuilder.AlterIndexTableAsync<WorkflowInstanceIndex>(table =>
        {
            table.CreateIndex("IDX_WorkflowInstanceIndex_InstanceId", nameof(WorkflowInstanceIndex.InstanceId));
            table.CreateIndex("IDX_WorkflowInstanceIndex_DefinitionId", nameof(WorkflowInstanceIndex.DefinitionId));
            table.CreateIndex("IDX_WorkflowInstanceIndex_DefinitionVersionId", nameof(WorkflowInstanceIndex.DefinitionVersionId));
            table.CreateIndex("IDX_WorkflowInstanceIndex_Version", nameof(WorkflowInstanceIndex.Version));
            table.CreateIndex("IDX_WorkflowInstanceIndex_Name", nameof(WorkflowInstanceIndex.Name));
            table.CreateIndex("IDX_WorkflowInstanceIndex_CorrelationId", nameof(WorkflowInstanceIndex.CorrelationId));
            table.CreateIndex("IDX_WorkflowInstanceIndex_ParentInstanceId", nameof(WorkflowInstanceIndex.ParentInstanceId));
            table.CreateIndex("IDX_WorkflowInstanceIndex_Status", nameof(WorkflowInstanceIndex.Status));
            table.CreateIndex("IDX_WorkflowInstanceIndex_SubStatus", nameof(WorkflowInstanceIndex.SubStatus));
            table.CreateIndex("IDX_WorkflowInstanceIndex_HasIncidents", nameof(WorkflowInstanceIndex.HasIncidents));
            table.CreateIndex("IDX_WorkflowInstanceIndex_IncidentCount", nameof(WorkflowInstanceIndex.IncidentCount));
            table.CreateIndex("IDX_WorkflowInstanceIndex_IsSystem", nameof(WorkflowInstanceIndex.IsSystem));
            table.CreateIndex("IDX_WorkflowInstanceIndex_CreatedAt", nameof(WorkflowInstanceIndex.CreatedAt));
            table.CreateIndex("IDX_WorkflowInstanceIndex_UpdatedAt", nameof(WorkflowInstanceIndex.UpdatedAt));
            table.CreateIndex("IDX_WorkflowInstanceIndex_FinishedAt", nameof(WorkflowInstanceIndex.FinishedAt));
        }, Collection);

        return 1;
    }
}