using Elsa.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;

namespace OrchardCore.Elsa.Features;

[Feature("OrchardCore.Elsa.Http")]
public class HttpStartup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureElsa(elsa =>
        {
            elsa.UseHttp(http =>
            {
                http.ConfigureHttpOptions = options =>
                {
                    options.BasePath = "/wf";
                    options.BaseUrl = new("https://localhost:8096");
                };
                http.UseCache();
            });
        });

    }

    public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
    {
        app.UseWorkflows();
    }
}