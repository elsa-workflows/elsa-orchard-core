using System.Threading.Tasks;
using JetBrains.Annotations;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using OrchardCore.ElsaWorkflows.Indexes;
using YesSql.Sql;

namespace OrchardCore.ElsaWorkflows.Migrations;

[UsedImplicitly]
public class WorkflowDefinitionMigrations(IContentDefinitionManager contentDefinitionManager) : DataMigration
{
    private const string Collection = ElsaCollections.WorkflowDefinitions;
    
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
        , Collection);

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
        }, Collection);

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