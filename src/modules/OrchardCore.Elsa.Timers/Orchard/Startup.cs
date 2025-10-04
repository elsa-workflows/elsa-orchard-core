using Elsa.Extensions;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.BackgroundTasks;
using OrchardCore.Elsa.Timers.Orchard.Activities;
using OrchardCore.Elsa.Timers.Orchard.Tasks;
using OrchardCore.Modules;

namespace OrchardCore.Elsa.Timers.Orchard;

[Feature("OrchardCore.Elsa.Timers.Orchard")]
public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureElsa(elsa =>
        {
            elsa.AddActivity<OrchardCron>();
        });   
        
        services.AddSingleton<IBackgroundTask, TimerBackgroundTask>();
    }
}