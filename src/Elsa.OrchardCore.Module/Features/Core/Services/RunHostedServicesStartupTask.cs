using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using OrchardCore.Modules;

namespace Elsa.OrchardCore.Features.Core.Services;

// Run Elsa, and some others, hosted services.
public class RunHostedServicesStartupTask : ModularTenantEvents
{
    private readonly IEnumerable<IHostedService> _hostedServices;

    public RunHostedServicesStartupTask(IEnumerable<IHostedService> hostedServices)
    {
        _hostedServices = Filter(hostedServices);
    }

    public override async Task ActivatingAsync()
    {
        foreach (var hostedService in _hostedServices)
        {
            await hostedService.StartAsync(CancellationToken.None);
        }
    }

    public override async Task TerminatingAsync()
    {
        foreach (var hostedService in _hostedServices)
        {
            await hostedService.StopAsync(CancellationToken.None);
        }
    }

    private static IEnumerable<IHostedService> Filter(IEnumerable<IHostedService> hostedServices)
    {
        foreach (var hostedService in hostedServices)
        {
            var hostedServiceType = hostedService.GetType();

            if (hostedServiceType.Namespace?.StartsWith("Elsa") == true)
                yield return hostedService;

            if (hostedServiceType.Namespace?.StartsWith("Quartz") == true)
                yield return hostedService;
        }
    }
}