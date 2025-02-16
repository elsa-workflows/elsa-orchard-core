using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OrchardCore.Data.Migration;
using OrchardCore.ElsaWorkflows.Indexes;
using YesSql.Sql;

namespace OrchardCore.ElsaWorkflows.Migrations;

[UsedImplicitly]
public class WorkflowExecutionLogRecordMigrations : DataMigration
{
    private const string Collection = ElsaCollections.WorkflowExecutionLogRecords;

    [UsedImplicitly]
    public async Task<int> CreateAsync()
    {
        await SchemaBuilder.CreateMapIndexTableAsync<WorkflowExecutionLogRecordIndex>(table => table
                .Column<string>(nameof(WorkflowExecutionLogRecordIndex.RecordId), c => c.NotNull().WithLength(32))
                .Column<string>(nameof(WorkflowExecutionLogRecordIndex.WorkflowInstanceId), c => c.NotNull().WithLength(32))
                .Column<string>(nameof(WorkflowExecutionLogRecordIndex.ParentActivityInstanceId), c => c.Nullable().WithLength(32))
                .Column<string>(nameof(WorkflowExecutionLogRecordIndex.ActivityId), c => c.NotNull())
                .Column<string>(nameof(WorkflowExecutionLogRecordIndex.ActivityNodeId), c => c.NotNull())
                .Column<string>(nameof(WorkflowExecutionLogRecordIndex.EventName), c => c.NotNull())
                .Column<DateTimeOffset>(nameof(WorkflowExecutionLogRecordIndex.Timestamp), c => c.NotNull())
                .Column<long>(nameof(WorkflowExecutionLogRecordIndex.Sequence), c => c.NotNull())
            , Collection);

        await SchemaBuilder.AlterIndexTableAsync<WorkflowExecutionLogRecordIndex>(table =>
        {
            table.CreateIndex("IDX_WorkflowExecutionLogRecordIndex_RecordId", nameof(WorkflowExecutionLogRecordIndex.RecordId));
            table.CreateIndex("IDX_WorkflowExecutionLogRecordIndex_WorkflowInstanceId", nameof(WorkflowExecutionLogRecordIndex.WorkflowInstanceId));
            table.CreateIndex("IDX_WorkflowExecutionLogRecordIndex_ParentActivityInstanceId", nameof(WorkflowExecutionLogRecordIndex.ParentActivityInstanceId));
            table.CreateIndex("IDX_WorkflowExecutionLogRecordIndex_ActivityId", nameof(WorkflowExecutionLogRecordIndex.ActivityId));
            table.CreateIndex("IDX_WorkflowExecutionLogRecordIndex_ActivityNodeId", nameof(WorkflowExecutionLogRecordIndex.ActivityNodeId));
            table.CreateIndex("IDX_WorkflowExecutionLogRecordIndex_EventName", nameof(WorkflowExecutionLogRecordIndex.EventName));
            table.CreateIndex("IDX_WorkflowExecutionLogRecordIndex_Timestamp", nameof(WorkflowExecutionLogRecordIndex.Timestamp));
            table.CreateIndex("IDX_WorkflowExecutionLogRecordIndex_Sequence", nameof(WorkflowExecutionLogRecordIndex.Sequence));
        }, Collection);

        return 1;
    }
}