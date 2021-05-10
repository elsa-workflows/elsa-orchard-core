using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.OrchardCore.Models;

namespace Elsa.OrchardCore.Services
{
    public interface IWorkflowServerStore
    {
        Task<IEnumerable<WorkflowServer>> ListAsync(CancellationToken cancellationToken = default);
        Task<WorkflowServer> SaveAsync(WorkflowServer workflowServer, CancellationToken cancellationToken = default);
        Task<WorkflowServer?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task DeleteAsync(WorkflowServer workflowServer, CancellationToken cancellationToken = default);
    }
}