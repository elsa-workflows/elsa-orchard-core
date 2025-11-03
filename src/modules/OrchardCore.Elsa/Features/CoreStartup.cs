using Elsa.Common.Multitenancy.HostedServices;
using Elsa.Extensions;
using Elsa.Mediator.HostedServices;
using Elsa.Resilience.Extensions;
using Elsa.Workflows.Runtime.Distributed.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.Data;
using OrchardCore.Data.Migration;
using OrchardCore.Elsa.Handlers.Content;
using OrchardCore.Elsa.Indexes;
using OrchardCore.Elsa.Migrations;
using OrchardCore.Elsa.Security;
using OrchardCore.Elsa.Services;
using OrchardCore.Elsa.StartupTasks;
using OrchardCore.Elsa.Stores;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.Users.Services;

namespace OrchardCore.Elsa.Features;

[Feature("OrchardCore.Elsa")]
public class CoreStartup : StartupBase
{
    public override int Order => int.MaxValue; // Run after all other modules.
    
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddElsa(elsa =>
        {
            elsa.AddActivitiesFrom<CoreStartup>();
            elsa.UseWorkflowManagement(workflowManagement =>
            {
                workflowManagement.UseWorkflowDefinitionPublisher(sp => ActivatorUtilities.CreateInstance<ContentItemWorkflowDefinitionPublisher>(sp));
                workflowManagement.UseWorkflowDefinitions(workflowDefinitions => workflowDefinitions.WorkflowDefinitionStore = sp => ActivatorUtilities.CreateInstance<ElsaWorkflowDefinitionStore>(sp));
                workflowManagement.UseWorkflowInstances(workflowInstances => workflowInstances.WorkflowInstanceStore = sp => ActivatorUtilities.CreateInstance<ElsaWorkflowInstanceStore>(sp));
                workflowManagement.UseCache();
            });
            elsa.UseWorkflowRuntime(workflowRuntime =>
            {
                workflowRuntime.TriggerStore = sp => ActivatorUtilities.CreateInstance<ElsaTriggerStore>(sp);
                workflowRuntime.BookmarkStore = sp => ActivatorUtilities.CreateInstance<ElsaBookmarkStore>(sp);
                workflowRuntime.WorkflowExecutionLogStore = sp => ActivatorUtilities.CreateInstance<ElsaWorkflowExecutionLogStore>(sp);
                workflowRuntime.ActivityExecutionLogStore = sp => ActivatorUtilities.CreateInstance<ElsaActivityExecutionRecordStore>(sp);
                workflowRuntime.UseDistributedRuntime();
                workflowRuntime.UseCache();
            });
            elsa.UseJavaScript();
            elsa.UseLiquid();
            elsa.UseResilience();
            elsa.UseWorkflowsApi(api => api.AddFastEndpointsAssembly<CoreStartup>());
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
            .AddSingleton<JobRunnerHostedService>()
            .AddSingleton<ActivateTenants>()
            .AddScoped<INavigationProvider, AdminMenu>()
            .AddScoped<IModularTenantEvents, PopulateRegistriesTask>()
            .AddScoped<IModularTenantEvents, StartHostedServices>()
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
            ;

        services.Configure<StaticFileOptions>(options =>
        {
            var provider = options.ContentTypeProvider as FileExtensionContentTypeProvider
                           ?? new FileExtensionContentTypeProvider();

            provider.Mappings[".pdb"]  = "application/octet-stream";
            provider.Mappings[".wasm"] = "application/wasm";
            provider.Mappings[".dat"]  = "application/octet-stream";

            options.ContentTypeProvider = provider;
        });
    }

    public override ValueTask ConfigureAsync(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
    {
        app.UseBlazorFrameworkFiles();
        routes.MapWorkflowsApi();
        
        return ValueTask.CompletedTask;
    }
}