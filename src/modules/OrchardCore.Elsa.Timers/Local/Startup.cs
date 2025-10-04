using Elsa.Common.Multitenancy;
using Elsa.Extensions;
using Elsa.Scheduling;
using Elsa.Scheduling.Bookmarks;
using Elsa.Scheduling.Features;
using Elsa.Scheduling.Handlers;
using Elsa.Scheduling.Services;
using Elsa.Scheduling.TriggerPayloadValidators;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Elsa.Timers.Local.Services;
using OrchardCore.Elsa.Timers.Local.Tasks;
using OrchardCore.Modules;

namespace OrchardCore.Elsa.Timers.Local;

[Feature("OrchardCore.Elsa.Timers.Local")]
public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureElsa(elsa =>
        {
            elsa.AddActivitiesFrom<SchedulingFeature>();
        });   
        
        services
            .AddSingleton<UpdateTenantSchedules>()
            .AddSingleton<ITenantActivatedEvent>(sp => sp.GetRequiredService<UpdateTenantSchedules>())
            .AddSingleton<ITenantDeletedEvent>(sp => sp.GetRequiredService<UpdateTenantSchedules>())
            .AddSingleton<IScheduler, LocalScheduler>()
            .AddSingleton<CronosCronParser>()
            .AddSingleton<ICronParser, CrontabCronParser>()
            .AddSingleton<IModularTenantEvents, CreateSchedulesBackgroundTask>()
            .AddScoped<ITriggerScheduler, DefaultTriggerScheduler>()
            .AddScoped<IBookmarkScheduler, DefaultBookmarkScheduler>()
            .AddScoped<DefaultWorkflowScheduler>()
            .AddScoped<IWorkflowScheduler, DefaultWorkflowScheduler>()
            .AddHandlersFrom<ScheduleWorkflows>()
            .AddTriggerPaylodValidator<CronTriggerPayloadValidator, CronTriggerPayload>()
            ;
    }
}