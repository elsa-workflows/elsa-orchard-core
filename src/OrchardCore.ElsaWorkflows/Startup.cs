using System;
using System.Threading.Tasks;
using Elsa.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.Data;
using OrchardCore.Data.Migration;
using OrchardCore.ElsaWorkflows.Handlers.Content;
using OrchardCore.ElsaWorkflows.Indexes;
using OrchardCore.ElsaWorkflows.Security;
using OrchardCore.ElsaWorkflows.Services;
using OrchardCore.ElsaWorkflows.StartupTasks;
using OrchardCore.ElsaWorkflows.Stores;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.Users.Services;

namespace OrchardCore.ElsaWorkflows;

public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddElsa(elsa =>
        {
            elsa.UseWorkflowManagement(workflowManagement =>
            {
                workflowManagement.WithWorkflowDefinitionPublisher(sp => ActivatorUtilities.CreateInstance<ContentItemWorkflowDefinitionPublisher>(sp));
                workflowManagement.UseWorkflowDefinitions(workflowDefinitions => workflowDefinitions.WorkflowDefinitionStore = sp => ActivatorUtilities.CreateInstance<ContentItemWorkflowDefinitionStore>(sp));
            });
            elsa.UseWorkflowRuntime();
            elsa.UseWorkflowsApi(api => api.AddFastEndpointsAssembly<Startup>());
        });
        
        services
            .AddDataMigration<Migrations>()
            .AddScoped<INavigationProvider, AdminMenu>()
            .AddScoped<IModularTenantEvents, PopulateRegistriesTask>()
            .AddScoped<IContentHandler, WorkflowDefinitionContentHandler>()
            .AddScoped<IUserClaimsProvider, PermissionsClaimsProvider>()
            .AddScoped<WorkflowDefinitionPartMapper>()
            .AddScoped<WorkflowDefinitionPartSerializer>()
            .AddIndexProvider<WorkflowDefinitionIndexProvider>()
            .Configure<StaticFileOptions>(ConfigureStaticFileOptions);
    }

    public override ValueTask ConfigureAsync(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
    {
        routes.MapWorkflowsApi();
        return ValueTask.CompletedTask;
    }

    private void ConfigureStaticFileOptions(StaticFileOptions options)
    {
        var provider = new FileExtensionContentTypeProvider
        {
            Mappings =
            {
                // Add custom MIME type mappings for WASM
                [".dat"] = "application/octet-stream" // Adjust the MIME type as needed
            }
        };
        options.ContentTypeProvider = provider;
    }
}