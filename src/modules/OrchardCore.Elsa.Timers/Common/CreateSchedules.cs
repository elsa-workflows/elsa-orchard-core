using Elsa.Scheduling;
using Elsa.Workflows.Runtime;
using Elsa.Workflows.Runtime.Entities;
using Elsa.Workflows.Runtime.Filters;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Environment.Shell.Scope;
using OrchardCore.Modules;

namespace OrchardCore.Elsa.Timers.Common;

/// <summary>
/// Creates new schedules when using the default scheduler (which doesn't have its own persistence layer like Quartz or Hangfire).
/// </summary>
[UsedImplicitly]
public class CreateSchedules(IServiceScopeFactory scopeFactory) : ModularTenantEvents
{
    public override Task ActivatedAsync()
    {
        ShellScope.AddDeferredTask(async scope =>
        {
            using var serviceScope = scopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var (triggers, bookmarks) = await GetTriggersAndBookmarksAsync(serviceProvider);
            var triggerScheduler = serviceProvider.GetRequiredService<ITriggerScheduler>();
            var bookmarkScheduler = serviceProvider.GetRequiredService<IBookmarkScheduler>();
            await triggerScheduler.ScheduleAsync(triggers);
            await bookmarkScheduler.ScheduleAsync(bookmarks);
        });
        return Task.CompletedTask;
    }

    public override async Task TerminatingAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var (triggers, bookmarks) = await GetTriggersAndBookmarksAsync(serviceProvider);
        var triggerScheduler = serviceProvider.GetRequiredService<ITriggerScheduler>();
        var bookmarkScheduler = serviceProvider.GetRequiredService<IBookmarkScheduler>();
        await triggerScheduler.UnscheduleAsync(triggers);
        await bookmarkScheduler.UnscheduleAsync(bookmarks);
    }

    private async Task<(List<StoredTrigger>, List<StoredBookmark>)> GetTriggersAndBookmarksAsync(IServiceProvider serviceProvider)
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
        
        var triggerStore = serviceProvider.GetRequiredService<ITriggerStore>();
        var bookmarkStore = serviceProvider.GetRequiredService<IBookmarkStore>();
        var triggers = (await triggerStore.FindManyAsync(triggerFilter)).ToList();
        var bookmarks = (await bookmarkStore.FindManyAsync(bookmarkFilter)).ToList();
        
        return (triggers, bookmarks);
    }
}