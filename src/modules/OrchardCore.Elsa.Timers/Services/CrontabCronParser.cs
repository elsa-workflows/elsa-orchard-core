using Elsa.Scheduling;
using OrchardCore.Modules;

namespace OrchardCore.Elsa.Timers.Services;

public class CrontabCronParser(IClock clock) : ICronParser
{
    /// <inheritdoc />
    public DateTimeOffset GetNextOccurrence(string expression)
    {
        var options = new NCrontab.CrontabSchedule.ParseOptions { IncludingSeconds = true };
        var schedule = NCrontab.CrontabSchedule.Parse(expression, options);
        var now = clock.UtcNow;
        var next = schedule.GetNextOccurrence(now);
        return new(next);
    }
}