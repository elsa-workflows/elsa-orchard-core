using System.Threading.Tasks;
using JetBrains.Annotations;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using OrchardCore.ElsaWorkflows.Indexes;
using OrchardCore.Title.Models;
using YesSql.Sql;

namespace OrchardCore.ElsaWorkflows;

[UsedImplicitly]
public class Migrations(IContentDefinitionManager contentDefinitionManager) : DataMigration
{
    [UsedImplicitly]
    public async Task<int> CreateAsync()
    {
        await SchemaBuilder.CreateMapIndexTableAsync<WorkflowDefinitionIndex>(table => table
            .Column<string>(nameof(WorkflowDefinitionIndex.WorkflowDefinitionId), c => c.WithLength(32))
            .Column<string>(nameof(WorkflowDefinitionIndex.Name))
        );
        
        await SchemaBuilder.AlterIndexTableAsync<WorkflowDefinitionIndex>(table =>
        {
            table.CreateIndex("IDX_WorkflowDefinitionIndex_WorkflowDefinitionId", nameof(WorkflowDefinitionIndex.WorkflowDefinitionId));
            table.CreateIndex("IDX_WorkflowDefinitionIndex_Name", nameof(WorkflowDefinitionIndex.Name));
        });
        
        await contentDefinitionManager.AlterPartDefinitionAsync("WorkflowDefinitionPart", part => part
            .Attachable()
            .WithDisplayName("Workflow Definition")
            .WithDescription("Turns your content item into a workflow definition."));

        await contentDefinitionManager.AlterTypeDefinitionAsync("WorkflowDefinition", type => type
            .WithPart("TitlePart", part => part
                .WithSettings(new TitlePartSettings { RenderTitle = false })
                .WithPosition("0")
            )
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