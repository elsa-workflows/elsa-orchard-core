using Elsa.Scheduling;
using Elsa.Scheduling.Quartz.Contracts;
using Elsa.Scheduling.Quartz.Handlers;
using Elsa.Scheduling.Quartz.Services;
using Elsa.Workflows;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;

namespace OrchardCore.Elsa.Timers.Quartz;

[Feature("OrchardCore.Elsa.Timers.Quartz")]
public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services
            .AddSingleton<IActivityDescriptorModifier, CronActivityDescriptorModifier>()
            .AddSingleton<ICronParser, QuartzCronParser>()
            .AddScoped<QuartzWorkflowScheduler>()
            .AddScoped<IJobKeyProvider, JobKeyProvider>()
            .AddSingleton<IModularTenantEvents, RegisterJobs>();
    }
}