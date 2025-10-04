using Elsa.Scheduling.Quartz.Contracts;
using Elsa.Scheduling.Quartz.Jobs;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;
using Quartz;
using QuartzIScheduler = Quartz.IScheduler;

namespace OrchardCore.Elsa.Timers.Quartz;

/// <summary>
/// Registers the Quartz jobs.
/// </summary>
[UsedImplicitly]
internal class RegisterJobs(ISchedulerFactory schedulerFactoryFactory, IServiceScopeFactory scopeFactory) : ModularTenantEvents
{
    public override async Task ActivatingAsync()
    {
        var scheduler = await schedulerFactoryFactory.GetScheduler();
        await CreateJobAsync<RunWorkflowJob>(scheduler);
        await CreateJobAsync<ResumeWorkflowJob>(scheduler);
    }
    
    private async Task CreateJobAsync<TJobType>(QuartzIScheduler scheduler, CancellationToken cancellationToken = default) where TJobType : IJob
    {
        using var scope = scopeFactory.CreateScope();
        var jobKeyProvider = scope.ServiceProvider.GetRequiredService<IJobKeyProvider>();
        var key = jobKeyProvider.GetJobKey<TJobType>();
        var job = JobBuilder.Create<TJobType>()
            .WithIdentity(key)
            .StoreDurably()
            .Build();
        
        if (!await scheduler.CheckExists(job.Key, cancellationToken))
            await scheduler.AddJob(job, false, cancellationToken);
    }
}