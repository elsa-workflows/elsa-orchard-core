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
using OrchardCore.ElsaWorkflows.Migrations;
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
    public override int Order => int.MaxValue; // Run after all other modules.
    
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddElsa(elsa =>
        {
            elsa.UseWorkflowManagement(workflowManagement =>
            {
                workflowManagement.WithWorkflowDefinitionPublisher(sp => ActivatorUtilities.CreateInstance<ContentItemWorkflowDefinitionPublisher>(sp));
                workflowManagement.UseWorkflowDefinitions(workflowDefinitions => workflowDefinitions.WorkflowDefinitionStore = sp => ActivatorUtilities.CreateInstance<ElsaWorkflowDefinitionStore>(sp));
                workflowManagement.UseWorkflowInstances(workflowInstances => workflowInstances.WorkflowInstanceStore = sp => ActivatorUtilities.CreateInstance<ElsaWorkflowInstanceStore>(sp));
            });
            elsa.UseWorkflowRuntime(workflowRuntime =>
            {
                workflowRuntime.TriggerStore = sp => ActivatorUtilities.CreateInstance<ElsaTriggerStore>(sp);
                workflowRuntime.BookmarkStore = sp => ActivatorUtilities.CreateInstance<ElsaBookmarkStore>(sp);
                workflowRuntime.WorkflowExecutionLogStore = sp => ActivatorUtilities.CreateInstance<ElsaWorkflowExecutionLogStore>(sp);
                workflowRuntime.ActivityExecutionLogStore = sp => ActivatorUtilities.CreateInstance<ElsaActivityExecutionRecordStore>(sp);
            });
            elsa.UseJavaScript();
            elsa.UseLiquid();
            elsa.UseWorkflowsApi(api => api.AddFastEndpointsAssembly<Startup>());
        });
        
        services.Configure<StoreCollectionOptions>(o =>
        {
            o.Collections.Add(ElsaCollections.WorkflowInstances);
            o.Collections.Add(ElsaCollections.StoredTriggers);
            o.Collections.Add(ElsaCollections.StoredBookmarks);
            o.Collections.Add(ElsaCollections.WorkflowExecutionLogRecords);
            o.Collections.Add(ElsaCollections.ActivityExecutionRecords);
        });
        
        services
            .AddDataMigration<WorkflowDefinitionMigrations>()
            .AddDataMigration<WorkflowInstanceMigrations>()
            .AddDataMigration<StoredTriggerMigrations>()
            .AddDataMigration<StoredBookmarkMigrations>()
            .AddDataMigration<WorkflowExecutionLogRecordMigrations>()
            .AddDataMigration<ActivityExecutionRecordMigrations>()
            .AddScoped<INavigationProvider, AdminMenu>()
            .AddScoped<IModularTenantEvents, PopulateRegistriesTask>()
            .AddScoped<IContentHandler, WorkflowDefinitionContentHandler>()
            .AddScoped<IUserClaimsProvider, PermissionsClaimsProvider>()
            .AddScoped<WorkflowDefinitionPartMapper>()
            .AddScoped<WorkflowDefinitionPartSerializer>()
            .AddIndexProvider<WorkflowDefinitionIndexProvider>()
            .AddIndexProvider<WorkflowInstanceIndexProvider>()
            .AddIndexProvider<StoredTriggerIndexProvider>()
            .AddIndexProvider<StoredBookmarkIndexProvider>()
            .AddIndexProvider<WorkflowExecutionLogRecordIndexProvider>()
            .AddIndexProvider<ActivityExecutionRecordIndexProvider>()
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