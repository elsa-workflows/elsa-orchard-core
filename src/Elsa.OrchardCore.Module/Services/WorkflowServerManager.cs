using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.OrchardCore.Contracts;

namespace Elsa.OrchardCore.Services;

public class WorkflowServerManager : IWorkflowServerManager
{
    private readonly IEnumerable<IWorkflowServerProvider> _providers;

    public WorkflowServerManager(IEnumerable<IWorkflowServerProvider> providers) => _providers = providers;

    public async Task<IEnumerable<IWorkflowServer>> ListServersAsync(CancellationToken cancellationToken = default)
    {
        var servers = await Task.WhenAll(_providers.Select(async x => await x.ListWorkflowServersAsync(cancellationToken)));

        return servers.SelectMany(x => x);
    }

    public async Task<IWorkflowServer?> GetServerAsync(string id, CancellationToken cancellationToken = default)
    {
        var servers = await ListServersAsync(cancellationToken);
        return servers.FirstOrDefault(x => x.Id == id);
    }
}