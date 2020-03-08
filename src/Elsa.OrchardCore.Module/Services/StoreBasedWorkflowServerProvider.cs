using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.OrchardCore.Models;

namespace Elsa.OrchardCore.Services
{
    public class StoreBasedWorkflowServerProvider : IWorkflowServerProvider
    {
        private readonly IWorkflowServerStore _store;
        public StoreBasedWorkflowServerProvider(IWorkflowServerStore store) => _store = store;
        public Task<IEnumerable<WorkflowServer>> ListWorkflowServersAsync(CancellationToken cancellationToken = default) => _store.ListAsync(cancellationToken);
    }
}