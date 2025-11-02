using Elsa.Common.Multitenancy.HostedServices;
using Elsa.Mediator.HostedServices;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Environment.Shell.Scope;
using OrchardCore.Modules;

namespace OrchardCore.Elsa.StartupTasks;

public class StartHostedServices : ModularTenantEvents
{
    public override Task ActivatedAsync()
    {
        ShellScope.AddDeferredTask(async scope =>
        {
            var activateTenants = scope.ServiceProvider.GetRequiredService<ActivateTenants>();
            var jobRunnerHostedService = scope.ServiceProvider.GetRequiredService<JobRunnerHostedService>();
            await activateTenants.StartAsync(CancellationToken.None);
            await jobRunnerHostedService.StartAsync(CancellationToken.None);    
        });
        return Task.CompletedTask;
    }

    public override async Task TerminatingAsync()
    {
        var activateTenants = ShellScope.Services.GetRequiredService<ActivateTenants>();
        var jobRunnerHostedService = ShellScope.Services.GetRequiredService<JobRunnerHostedService>();
        await activateTenants.StopAsync(CancellationToken.None);
        await jobRunnerHostedService.StopAsync(CancellationToken.None);
    }
}