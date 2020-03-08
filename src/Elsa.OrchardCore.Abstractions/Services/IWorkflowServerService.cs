using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.OrchardCore.Models;

namespace Elsa.OrchardCore.Services
{
    public interface IWorkflowServerService
    {
        Task<IEnumerable<WorkflowServer>> ListWorkflowServersAsync(CancellationToken cancellationToken = default);
        Task<WorkflowServer?> GetWorkflowServerAsync(string id, CancellationToken cancellationToken = default);
    }
}