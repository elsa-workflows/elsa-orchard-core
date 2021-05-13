using System.Threading;
using System.Threading.Tasks;
using Elsa.Client.Models;
using Elsa.OrchardCore.Services;

namespace Elsa.OrchardCore.Features.LocalWorkflowServer.Services
{
    public class LocalWorkflowServerClient : IWorkflowServerClient
    {
        public Task<PagedList<WorkflowDefinitionSummary>> ListWorkflowDefinitionsAsync(int? page = default, int? pageSize = default, VersionOptions? version = default, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<WorkflowDefinition?> GetWorkflowDefinitionAsync(string definitionId, VersionOptions version, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<WorkflowDefinition?> GetWorkflowDefinitionAsync(string id, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<WorkflowDefinition> SaveWorkflowDefinitionAsync(SaveWorkflowDefinitionRequest request, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteWorkflowDefinitionAsync(string definitionId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}