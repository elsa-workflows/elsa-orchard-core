using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.OrchardCore.Models;

namespace Elsa.OrchardCore.Services
{
    public interface IWorkflowServerManager
    {
        Task<IEnumerable<IWorkflowServer>> ListWorkflowServersAsync(CancellationToken cancellationToken = default);
        Task<IWorkflowServer?> GetWorkflowServerAsync(string id, CancellationToken cancellationToken = default);
    }
}