using System.Threading;
using System.Threading.Tasks;
using Elsa.Client;
using Elsa.Client.Models;
using Elsa.OrchardCore.Services;

namespace Elsa.OrchardCore.Features.RemoteWorkflows.Services
{
    public class RemoteWorkflowServerClient : IWorkflowServerClient
    {
        private readonly IElsaClient _elsaClient;
        public RemoteWorkflowServerClient(IElsaClient elsaClient) => _elsaClient = elsaClient;

        public Task<PagedList<WorkflowDefinitionSummary>> ListWorkflowDefinitionsAsync(int? page = default, int? pageSize = default, VersionOptions? version = default, CancellationToken cancellationToken = default) =>
            _elsaClient.WorkflowDefinitions.ListAsync(page, pageSize, version, cancellationToken);

        public Task<WorkflowDefinition?> GetWorkflowDefinitionAsync(string definitionId, VersionOptions version, CancellationToken cancellationToken = default) =>
            _elsaClient.WorkflowDefinitions.GetByIdAsync(definitionId, version, cancellationToken);

        public Task<WorkflowDefinition?> GetWorkflowDefinitionAsync(string id, CancellationToken cancellationToken = default) => _elsaClient.WorkflowDefinitions.GetByVersionIdAsync(id, cancellationToken);
        public Task<WorkflowDefinition> SaveWorkflowDefinitionAsync(SaveWorkflowDefinitionRequest request, CancellationToken cancellationToken = default) => _elsaClient.WorkflowDefinitions.SaveAsync(request, cancellationToken);
        public Task DeleteWorkflowDefinitionAsync(string definitionId, CancellationToken cancellationToken) => _elsaClient.WorkflowDefinitions.DeleteAsync(definitionId, cancellationToken);
    }
}