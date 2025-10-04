using OrchardCore.BackgroundTasks;

namespace OrchardCore.Elsa.Timers.Orchard.Tasks;

[BackgroundTask(
    Title = "Timed Workflow Starter",
    Schedule = "* * * * *",
    Description = "Triggers timed workflow events.")]
public sealed class TimerBackgroundTask : IBackgroundTask
{
    public async Task DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        Console.WriteLine("TimerBackgroundTask executed.");
        
    }
}
