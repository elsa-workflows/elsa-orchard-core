using Elsa.Extensions;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;

namespace OrchardCore.Elsa.Data;

[Feature("OrchardCore.Elsa.Data.Csv")]
public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureElsa(elsa =>
        {
            elsa.UseCsv();
        });
    }
}