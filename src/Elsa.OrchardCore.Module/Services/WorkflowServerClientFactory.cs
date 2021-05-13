using System;
using System.Threading;
using System.Threading.Tasks;

namespace Elsa.OrchardCore.Services
{
    public class WorkflowServerClientFactory : IWorkflowServerClientFactory
    {
        private readonly IWorkflowServerManager _workflowServerManager;

        public WorkflowServerClientFactory(IWorkflowServerManager workflowServerManager)
        {
            _workflowServerManager = workflowServerManager;
        }
        
        public async Task<IWorkflowServerClient> CreateClientAsync(string serverId, CancellationToken cancellationToken = default)
        {
            var workflowServer = await _workflowServerManager.GetWorkflowServerAsync(serverId, cancellationToken);

            if (workflowServer == null)
                throw new Exception("Server not found");

            return workflowServer.CreateClient();
        }

        public Task<IWorkflowServerClient> CreateClientAsync(IWorkflowServer server, CancellationToken cancellationToken = default) => Task.FromResult(server.CreateClient());
    }
}