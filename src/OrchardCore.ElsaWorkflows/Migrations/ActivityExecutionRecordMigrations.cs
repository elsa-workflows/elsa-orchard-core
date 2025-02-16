using System;
using System.Threading.Tasks;
using Elsa.Workflows;
using JetBrains.Annotations;
using OrchardCore.Data.Migration;
using OrchardCore.ElsaWorkflows.Indexes;
using YesSql.Sql;

namespace OrchardCore.ElsaWorkflows.Migrations;

[UsedImplicitly]
public class ActivityExecutionRecordMigrations : DataMigration
{
    private const string Collection = ElsaCollections.ActivityExecutionRecords;

    [UsedImplicitly]
    public async Task<int> CreateAsync()
    {
        await SchemaBuilder.CreateMapIndexTableAsync<ActivityExecutionRecordIndex>(table => table
                .Column<string>(nameof(ActivityExecutionRecordIndex.RecordId), c => c.NotNull().WithLength(32))
                .Column<string>(nameof(ActivityExecutionRecordIndex.WorkflowInstanceId), c => c.NotNull().WithLength(32))
                .Column<string>(nameof(ActivityExecutionRecordIndex.ActivityId), c => c.NotNull())
                .Column<string>(nameof(ActivityExecutionRecordIndex.ActivityNodeId), c => c.NotNull())
                .Column<string>(nameof(ActivityExecutionRecordIndex.ActivityType), c => c.NotNull())
                .Column<int>(nameof(ActivityExecutionRecordIndex.ActivityTypeVersion), c => c.NotNull())
                .Column<string>(nameof(ActivityExecutionRecordIndex.ActivityName), c => c.Nullable())
                .Column<ActivityStatus>(nameof(ActivityExecutionRecordIndex.Status), c => c.NotNull())
                .Column<bool>(nameof(ActivityExecutionRecordIndex.Completed), c => c.NotNull())
                .Column<bool>(nameof(ActivityExecutionRecordIndex.HasBookmarks), c => c.NotNull())
                .Column<DateTimeOffset>(nameof(ActivityExecutionRecordIndex.StartedAt), c => c.NotNull())
                .Column<DateTimeOffset>(nameof(ActivityExecutionRecordIndex.CompletedAt), c => c.Nullable())
            , Collection);

        await SchemaBuilder.AlterIndexTableAsync<ActivityExecutionRecordIndex>(table =>
        {
            table.CreateIndex("IDX_ActivityExecutionRecordIndex_RecordId", nameof(ActivityExecutionRecordIndex.RecordId));
            table.CreateIndex("IDX_ActivityExecutionRecordIndex_WorkflowInstanceId", nameof(ActivityExecutionRecordIndex.WorkflowInstanceId));
            table.CreateIndex("IDX_ActivityExecutionRecordIndex_ActivityId", nameof(ActivityExecutionRecordIndex.ActivityId));
            table.CreateIndex("IDX_ActivityExecutionRecordIndex_ActivityNodeId", nameof(ActivityExecutionRecordIndex.ActivityNodeId));
            table.CreateIndex("IDX_ActivityExecutionRecordIndex_ActivityType", nameof(ActivityExecutionRecordIndex.ActivityType));
            table.CreateIndex("IDX_ActivityExecutionRecordIndex_ActivityTypeVersion", nameof(ActivityExecutionRecordIndex.ActivityTypeVersion));
            table.CreateIndex("IDX_ActivityExecutionRecordIndex_ActivityName", nameof(ActivityExecutionRecordIndex.ActivityName));
            table.CreateIndex("IDX_ActivityExecutionRecordIndex_Status", nameof(ActivityExecutionRecordIndex.Status));
            table.CreateIndex("IDX_ActivityExecutionRecordIndex_Completed", nameof(ActivityExecutionRecordIndex.Completed));
            table.CreateIndex("IDX_ActivityExecutionRecordIndex_HasBookmarks", nameof(ActivityExecutionRecordIndex.HasBookmarks));
            table.CreateIndex("IDX_ActivityExecutionRecordIndex_StartedAt", nameof(ActivityExecutionRecordIndex.StartedAt));
            table.CreateIndex("IDX_ActivityExecutionRecordIndex_CompletedAt", nameof(ActivityExecutionRecordIndex.CompletedAt));
        }, Collection);

        return 1;
    }
}