using System.Threading;
using System.Threading.Tasks;
using Elsa.Client.Models;

namespace Elsa.OrchardCore.Services
{
    public interface IWorkflowServerClient
    {
        Task<PagedList<WorkflowDefinitionSummary>> ListWorkflowDefinitionsAsync(int? page = default, int? pageSize = default, VersionOptions? version = default, CancellationToken cancellationToken = default);
        Task<WorkflowDefinition?> GetWorkflowDefinitionAsync(string definitionId, VersionOptions version, CancellationToken cancellationToken = default);
        Task<WorkflowDefinition?> GetWorkflowDefinitionAsync(string id, CancellationToken cancellationToken = default);
        Task<WorkflowDefinition> SaveWorkflowDefinitionAsync(SaveWorkflowDefinitionRequest request, CancellationToken cancellationToken = default);
        Task DeleteWorkflowDefinitionAsync(string definitionId, CancellationToken cancellationToken);
    }
}