using Elsa.Agents;
using Elsa.Extensions;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.Data;
using OrchardCore.Data.Migration;
using OrchardCore.Elsa.Agents.Migrations;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.Security.Permissions;

namespace OrchardCore.Elsa.Agents;

public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureElsa(elsa =>
        {
            elsa.UseAgentActivities();
            elsa.UseAgentPersistence(feature =>
            {
                feature
                    .UseAgentStore(sp => ActivatorUtilities.CreateInstance<Stores.ElsaAgentStore>(sp))
                    .UseApiKeyStore(sp => ActivatorUtilities.CreateInstance<Stores.ElsaApiKeyStore>(sp))
                    .UseServiceStore(sp => ActivatorUtilities.CreateInstance<Stores.ElsaServiceStore>(sp));
            });
        });
        
        services.Configure<StoreCollectionOptions>(options =>
        {
            options.Collections.Add(ElsaAgentCollections.AgentApiKeys);
            options.Collections.Add(ElsaAgentCollections.AgentServices);
        });
        
        services.AddContentPart<Parts.AgentPart>()
            .UseDisplayDriver<Drivers.AgentPartDriver>();

        services
            .AddDataMigration<AgentMigrations>()
            .AddScoped<IPermissionProvider, Permissions>()
            .AddScoped<INavigationProvider, AdminMenu>()
            .AddIndexProvider<Indexes.AgentIndexProvider>()
            .AddIndexProvider<Indexes.ApiKeyDefinitionIndexProvider>()
            .AddIndexProvider<Indexes.ServiceDefinitionIndexProvider>();
    }
}
