using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Entities;
using OrchardCore.Modules;
using OrchardCore.Navigation;

namespace Elsa.OrchardCore;

[PublicAPI]
public class Startup : StartupBase
{
    public Startup()
    {
        // TODO: Remove this once we have integrated Orchard security.
        EndpointSecurityOptions.DisableSecurity();
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        services
            .AddIdGeneration()
            .AddScoped<INavigationProvider, AdminMenu>();
    }
}