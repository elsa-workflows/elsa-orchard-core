using Elsa.Scheduling;
using Elsa.Workflows.Runtime;
using Elsa.Workflows.Runtime.Filters;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;

namespace OrchardCore.Elsa.Timers.Tasks;

/// <summary>
/// Creates new schedules when using the default scheduler (which doesn't have its own persistence layer like Quartz or Hangfire).
/// </summary>
[UsedImplicitly]
public class CreateSchedulesBackgroundTask(IServiceScopeFactory scopeFactory) : ModularTenantEvents
{
    public override async Task ActivatingAsync()
    {
        var stimulusNames = new[]
        {
            SchedulingStimulusNames.Cron, SchedulingStimulusNames.Timer, SchedulingStimulusNames.StartAt, SchedulingStimulusNames.Delay,
        };
        var triggerFilter = new TriggerFilter
        {
            Names = stimulusNames
        };
        var bookmarkFilter = new BookmarkFilter
        {
            Names = stimulusNames
        };
        
        using var scope = scopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var triggerStore = serviceProvider.GetRequiredService<ITriggerStore>();
        var bookmarkStore = serviceProvider.GetRequiredService<IBookmarkStore>();
        var triggers = (await triggerStore.FindManyAsync(triggerFilter)).ToList();
        var bookmarks = (await bookmarkStore.FindManyAsync(bookmarkFilter)).ToList();
        var triggerScheduler = serviceProvider.GetRequiredService<ITriggerScheduler>();
        var bookmarkScheduler = serviceProvider.GetRequiredService<IBookmarkScheduler>();
        await triggerScheduler.ScheduleAsync(triggers);
        await bookmarkScheduler.ScheduleAsync(bookmarks);
    }
}