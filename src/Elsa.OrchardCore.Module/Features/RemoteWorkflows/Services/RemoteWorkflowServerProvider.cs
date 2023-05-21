using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.OrchardCore.Contracts;

namespace Elsa.OrchardCore.Features.RemoteWorkflows.Services;

public class RemoteWorkflowServerProvider : IWorkflowServerProvider
{
    private readonly IRemoteWorkflowServerStore _store;
    public RemoteWorkflowServerProvider(IRemoteWorkflowServerStore store) => _store = store;
        
    public async ValueTask<IEnumerable<IWorkflowServer>> ListWorkflowServersAsync(CancellationToken cancellationToken = default)
    {
        var records = await _store.ListAsync(cancellationToken);

        return records.Select(x => new RemoteWorkflowServer(x.WorkflowServerId, x.Name, x.Url, x.ApiKey));
    }
}