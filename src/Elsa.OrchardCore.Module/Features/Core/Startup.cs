using System;
using Elsa.EntityFrameworkCore.Extensions;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;
using Elsa.OrchardCore.Contracts;
using Elsa.OrchardCore.Extensions;
using Elsa.OrchardCore.Features.Core.Services;
using Elsa.OrchardCore.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;

namespace Elsa.OrchardCore.Features.Core;

[Feature("Elsa.OrchardCore.Module")]
public class Startup : StartupBase
{
    public override int Order => 9000;

    public override void ConfigureServices(IServiceCollection services)
    {
        var elsaModule = services.ConfigureElsa(elsa =>
        {
            elsa.AddActivitiesFrom<Startup>();
            elsa.UseWorkflowManagement(management =>
            {
                management.UseEntityFrameworkCore(m => m.UseSqlite());
            });
            elsa.UseWorkflowRuntime(runtime =>
            {
                runtime.UseDefaultRuntime(dr => dr.UseEntityFrameworkCore(ef => ef.UseSqlite()));
                runtime.UseExecutionLogRecords(e => e.UseEntityFrameworkCore(ef => ef.UseSqlite()));
                runtime.UseDefaultWorkflowStateExporter();
            });
            elsa.UseWorkflowsApi();
            elsa.UseIdentity(identity =>
            {
                identity.TokenOptions = options => options.SigningKey = "unused-secret-key"; // TODO: Decouple token feature from Identity.
            });
            elsa.UseDefaultAuthentication();
            elsa.UseHttp(http => http.ConfigureHttpOptions = options => options.BasePath = "/wf");
            elsa.UseScheduling();
        });

        elsaModule.Apply();
        
        services.AddScoped<IModularTenantEvents, RunHostedServicesStartupTask>();
        services.AddScoped<IElsaServerUrlAccessor, DefaultElsaServerUrlAccessor>();
    }

    public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
    {
        routes.MapWorkflowsApi();
        app.UseWorkflows();
    }
}