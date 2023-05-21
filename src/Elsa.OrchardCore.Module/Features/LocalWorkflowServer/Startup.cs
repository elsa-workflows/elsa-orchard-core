using System;
using Elsa.EntityFrameworkCore.Extensions;
using Elsa.EntityFrameworkCore.Modules.Identity;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;
using Elsa.OrchardCore.Contracts;
using Elsa.OrchardCore.Features.LocalWorkflowServer.Activities.Contents.ContentItemCreated;
using Elsa.OrchardCore.Features.LocalWorkflowServer.Services;
using Elsa.OrchardCore.Features.LocalWorkflowServer.StartupTasks;
using Elsa.Workflows.Core.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.Modules;

namespace Elsa.OrchardCore.Features.LocalWorkflowServer;

[Feature("Elsa.OrchardCore.LocalWorkflowServer")]
public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddElsa(elsa =>
        {
            elsa.AddActivitiesFrom<Startup>();
            elsa.UseWorkflowManagement(management =>
            {
                management.UseEntityFrameworkCore(m => m.UseSqlite());
                management.AddVariableType<ContentItem>("Content");
            });
            elsa.UseWorkflowRuntime(runtime =>
            {
                runtime.UseDefaultRuntime(dr => dr.UseEntityFrameworkCore(ef => ef.UseSqlite()));
                runtime.UseExecutionLogRecords(e => e.UseEntityFrameworkCore(ef => ef.UseSqlite()));
                runtime.UseDefaultWorkflowStateExporter();
            });
            // elsa.UseIdentity(identity =>
            // {
            //     identity.TokenOptions = options => { options.SigningKey = "secret-signing-key"; };
            //     identity.UseAdminUserProvider();
            // });
            //elsa.UseDefaultAuthentication();
            elsa.UseWorkflowsApi();
            elsa.UseHttp();
        });

        services.AddScoped<IModularTenantEvents, RunMigrationsStartupTask<ManagementElsaDbContext>>();
        services.AddScoped<IModularTenantEvents, RunMigrationsStartupTask<RuntimeElsaDbContext>>();
        services.AddScoped<IWorkflowServerProvider, LocalWorkflowServerProvider>();
        services.AddScoped<IModularTenantEvents, RegisterDescriptorsStartupTask>();
        services.AddScoped<IActivityPropertyOptionsProvider, ContentTypeOptionsProvider>();
        services.AddScoped<IContentHandler, WorkflowContentItemHandler>();
    }

    public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
    {
        routes.MapWorkflowsApi();
    }
}