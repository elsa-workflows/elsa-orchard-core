using Elsa.Extensions;
using Elsa.Workflows;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Elsa.Queries.UI;
using OrchardCore.Modules;

namespace OrchardCore.Elsa.Queries;

public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureElsa(elsa =>
        {
            elsa.AddActivitiesFrom<Startup>();
        });

        services.AddScoped<IPropertyUIHandler, SqlCodeOptionsProvider>();
    }
}