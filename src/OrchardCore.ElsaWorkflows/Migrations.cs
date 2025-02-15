using System;
using System.Threading.Tasks;
using Elsa.Workflows;
using JetBrains.Annotations;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using OrchardCore.ElsaWorkflows.Indexes;
using YesSql.Sql;

namespace OrchardCore.ElsaWorkflows;

[UsedImplicitly]
public class Migrations(IContentDefinitionManager contentDefinitionManager) : DataMigration
{
    [UsedImplicitly]
    public async Task<int> CreateAsync()
    {
        await SchemaBuilder.CreateMapIndexTableAsync<WorkflowDefinitionIndex>(table => table
            .Column<string>(nameof(WorkflowDefinitionIndex.DefinitionId), c => c.NotNull().WithLength(32))
            .Column<string>(nameof(WorkflowDefinitionIndex.DefinitionVersionId), c => c.NotNull().WithLength(32))
            .Column<string>(nameof(WorkflowDefinitionIndex.Description))
            .Column<int>(nameof(WorkflowDefinitionIndex.Version))
            .Column<bool>(nameof(WorkflowDefinitionIndex.IsLatest))
            .Column<bool>(nameof(WorkflowDefinitionIndex.IsPublished))
            .Column<bool>(nameof(WorkflowDefinitionIndex.IsReadonly))
            .Column<bool>(nameof(WorkflowDefinitionIndex.IsSystem))
            .Column<string>(nameof(WorkflowDefinitionIndex.MaterializerName))
            .Column<bool>(nameof(WorkflowDefinitionIndex.UsableAsActivity))
            .Column<string>(nameof(WorkflowDefinitionIndex.Name), c => c.NotNull())
        );

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
        );

        await SchemaBuilder.AlterIndexTableAsync<WorkflowDefinitionIndex>(table =>
        {
            table.CreateIndex("IDX_WorkflowDefinitionIndex_DefinitionId", nameof(WorkflowDefinitionIndex.DefinitionId));
            table.CreateIndex("IDX_WorkflowDefinitionIndex_DefinitionVersionId", nameof(WorkflowDefinitionIndex.DefinitionVersionId));
            table.CreateIndex("IDX_WorkflowDefinitionIndex_UsableAsActivity", nameof(WorkflowDefinitionIndex.UsableAsActivity));
            table.CreateIndex("IDX_WorkflowDefinitionIndex_IsPublished", nameof(WorkflowDefinitionIndex.IsPublished));
            table.CreateIndex("IDX_WorkflowDefinitionIndex_IsLatest", nameof(WorkflowDefinitionIndex.IsLatest));
            table.CreateIndex("IDX_WorkflowDefinitionIndex_Version", nameof(WorkflowDefinitionIndex.Version));
            table.CreateIndex("IDX_WorkflowDefinitionIndex_IsSystem", nameof(WorkflowDefinitionIndex.IsSystem));
            table.CreateIndex("IDX_WorkflowDefinitionIndex_MaterializerName", nameof(WorkflowDefinitionIndex.MaterializerName));
            table.CreateIndex("IDX_WorkflowDefinitionIndex_Name", nameof(WorkflowDefinitionIndex.Name));
            table.CreateIndex("IDX_WorkflowDefinitionIndex_Description", nameof(WorkflowDefinitionIndex.Description));
        });
        
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
        });

        await contentDefinitionManager.AlterPartDefinitionAsync("WorkflowDefinitionPart", part => part
            .Attachable()
            .WithDisplayName("Workflow Definition")
            .WithDescription("Turns your content item into a workflow definition."));

        await contentDefinitionManager.AlterTypeDefinitionAsync("WorkflowDefinition", type => type
            .WithPart("CommonPart", part => part
                .WithPosition("10")
            )
            .WithPart("WorkflowDefinitionPart")
            .Versionable()
            .Draftable()
            .Creatable()
            .Listable()
            .Securable()
            .WithDescription("A workflow definition is the blueprint for an executable workflow."));

        return 1;
    }
}