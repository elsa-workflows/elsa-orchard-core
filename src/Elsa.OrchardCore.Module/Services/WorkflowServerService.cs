using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.OrchardCore.Models;

namespace Elsa.OrchardCore.Services
{
    public class WorkflowServerService : IWorkflowServerService
    {
        private readonly IEnumerable<IWorkflowServerProvider> _providers;

        public WorkflowServerService(IEnumerable<IWorkflowServerProvider> providers) => _providers = providers;

        public async Task<IEnumerable<WorkflowServer>> ListWorkflowServersAsync(CancellationToken cancellationToken = default)
        {
            var servers = await Task.WhenAll(_providers.Select(x => x.ListWorkflowServersAsync(cancellationToken)));

            return servers.SelectMany(x => x);
        }

        public async Task<WorkflowServer?> GetWorkflowServerAsync(string id, CancellationToken cancellationToken = default)
        {
            var servers = await ListWorkflowServersAsync(cancellationToken);
            return servers.FirstOrDefault(x => x.WorkflowServerId == id);
        }
    }
}