using JetBrains.Annotations;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using OrchardCore.Elsa.Agents.Indexes;
using OrchardCore.Elsa.Agents.Parts;
using YesSql.Sql;

namespace OrchardCore.Elsa.Agents.Migrations;

[UsedImplicitly]
public class AgentMigrations(IContentDefinitionManager contentDefinitionManager) : DataMigration
{
    private const string ContentCollection = null!;

    [UsedImplicitly]
    public async Task<int> CreateAsync()
    {
        await contentDefinitionManager.AlterPartDefinitionAsync(AgentConstants.AgentPartName, part => part
            .Attachable()
            .WithDescription("Stores the Elsa agent configuration for a content item."));

        await contentDefinitionManager.AlterTypeDefinitionAsync(AgentConstants.AgentContentType, type => type
            .DisplayedAs("Agent")
            .WithPart("CommonPart", part => part.WithPosition("0"))
            .WithPart(AgentConstants.AgentPartName)
            .Listable()
            .Draftable()
            .Creatable()
            .Versionable()
            .Securable()
            .WithDescription("Represents an Elsa agent definition."));

        await SchemaBuilder.CreateMapIndexTableAsync<AgentIndex>(table => table
                .Column<string>(nameof(AgentIndex.AgentId), column => column.NotNull().WithLength(64))
                .Column<string>(nameof(AgentIndex.ContentItemId), column => column.NotNull().WithLength(64))
                .Column<string>(nameof(AgentIndex.ContentItemVersionId), column => column.NotNull().WithLength(64))
                .Column<string>(nameof(AgentIndex.Name), column => column.NotNull())
                .Column<bool>(nameof(AgentIndex.Published))
                .Column<bool>(nameof(AgentIndex.Latest))
            , ContentCollection);

        await SchemaBuilder.AlterIndexTableAsync<AgentIndex>(table =>
        {
            table.CreateIndex("IDX_AgentIndex_AgentId", nameof(AgentIndex.AgentId));
            table.CreateIndex("IDX_AgentIndex_Name", nameof(AgentIndex.Name));
            table.CreateIndex("IDX_AgentIndex_Published", nameof(AgentIndex.Published));
            table.CreateIndex("IDX_AgentIndex_Latest", nameof(AgentIndex.Latest));
        }, ContentCollection);

        await SchemaBuilder.CreateMapIndexTableAsync<ApiKeyDefinitionIndex>(table => table
                .Column<string>(nameof(ApiKeyDefinitionIndex.ApiKeyId), column => column.NotNull().WithLength(64))
                .Column<string>(nameof(ApiKeyDefinitionIndex.Name), column => column.NotNull())
            , ElsaAgentCollections.AgentApiKeys);

        await SchemaBuilder.AlterIndexTableAsync<ApiKeyDefinitionIndex>(table =>
        {
            table.CreateIndex("IDX_AgentApiKey_Id", nameof(ApiKeyDefinitionIndex.ApiKeyId));
            table.CreateIndex("IDX_AgentApiKey_Name", nameof(ApiKeyDefinitionIndex.Name));
        }, ElsaAgentCollections.AgentApiKeys);

        await SchemaBuilder.CreateMapIndexTableAsync<ServiceDefinitionIndex>(table => table
                .Column<string>(nameof(ServiceDefinitionIndex.ServiceId), column => column.NotNull().WithLength(64))
                .Column<string>(nameof(ServiceDefinitionIndex.Name), column => column.NotNull())
                .Column<string>(nameof(ServiceDefinitionIndex.Type), column => column.NotNull())
            , ElsaAgentCollections.AgentServices);

        await SchemaBuilder.AlterIndexTableAsync<ServiceDefinitionIndex>(table =>
        {
            table.CreateIndex("IDX_AgentService_Id", nameof(ServiceDefinitionIndex.ServiceId));
            table.CreateIndex("IDX_AgentService_Name", nameof(ServiceDefinitionIndex.Name));
            table.CreateIndex("IDX_AgentService_Type", nameof(ServiceDefinitionIndex.Type));
        }, ElsaAgentCollections.AgentServices);

        return 1;
    }
}
