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
using OrchardCore.ElsaWorkflows.Handlers.Requests;
using OrchardCore.ElsaWorkflows.Indexes;
using OrchardCore.ElsaWorkflows.Security;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.Users.Services;

namespace OrchardCore.ElsaWorkflows;

public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services
            .AddDataMigration<Migrations>()
            .AddScoped<INavigationProvider, AdminMenu>()
            .AddScoped<IContentHandler, WorkflowDefinitionContentHandler>()
            .AddScoped<IUserClaimsProvider, PermissionsClaimsProvider>()
            .AddIndexProvider<WorkflowDefinitionIndexProvider>()
            .Configure<StaticFileOptions>(ConfigureStaticFileOptions);

        services.AddElsa(elsa =>
        {
            elsa.UseWorkflowManagement(workflowManagement =>
            {
                workflowManagement.UseWorkflowDefinitions(workflowDefinitions =>
                {
                    workflowDefinitions.FindWorkflowDefinitionHandler = typeof(FindWorkflowDefinitionHandler);
                    workflowDefinitions.FindLastVersionOfWorkflowDefinitionHandler = typeof(FindLastVersionOfWorkflowDefinitionHandler);
                    workflowDefinitions.FindLatestOrPublishedWorkflowDefinitionsHandler = typeof(FindLatestOrPublishedWorkflowDefinitionsHandler);
                    workflowDefinitions.SaveWorkflowDefinitionHandler = typeof(SaveWorkflowDefinitionHandler);
                });
            });
            elsa.UseWorkflowRuntime();
            elsa.UseWorkflowsApi(api => api.AddFastEndpointsAssembly<Startup>());
        });
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