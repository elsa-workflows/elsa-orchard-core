using System;
using Elsa.EntityFrameworkCore.Extensions;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;
using Elsa.OrchardCore.Features.Core.Activities.Contents.ContentItemCreated;
using Elsa.OrchardCore.Features.Core.Services;
using Elsa.Workflows.Core.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.Modules;

namespace Elsa.OrchardCore.Features.Core;

[Feature("Elsa.OrchardCore.Module")]
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
            elsa.UseWorkflowsApi();
            elsa.UseHttp(http => http.ConfigureHttpOptions = options => options.BasePath = "/wf");
            elsa.UseScheduling();
        });

        services.AddScoped<IModularTenantEvents, RunHostedServicesStartupTask>();
        services.AddScoped<IActivityPropertyOptionsProvider, ContentTypeOptionsProvider>();
        services.AddScoped<IContentHandler, WorkflowContentItemHandler>();
    }

    public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
    {
        routes.MapWorkflowsApi();
        app.UseWorkflows();
    }
}