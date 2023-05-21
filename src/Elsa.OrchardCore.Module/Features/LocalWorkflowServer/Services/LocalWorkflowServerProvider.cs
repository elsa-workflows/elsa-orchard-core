using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.OrchardCore.Contracts;
using OrchardCore.Settings;

namespace Elsa.OrchardCore.Features.LocalWorkflowServer.Services;

public class LocalWorkflowServerProvider : IWorkflowServerProvider
{
    private readonly ISiteService _siteService;

    public LocalWorkflowServerProvider(ISiteService siteService) => _siteService = siteService;

    public ValueTask<IEnumerable<IWorkflowServer>> ListWorkflowServersAsync(CancellationToken cancellationToken = default)
    {
        var server = new LocalWorkflowServer("local", "Local", _siteService);
        return new ValueTask<IEnumerable<IWorkflowServer>>(new[] {server});
    }
}