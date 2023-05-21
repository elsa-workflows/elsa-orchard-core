using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Elsa.OrchardCore.Contracts
{
    public interface IWorkflowServerManager
    {
        Task<IEnumerable<IWorkflowServer>> ListServersAsync(CancellationToken cancellationToken = default);
        Task<IWorkflowServer?> GetServerAsync(string id, CancellationToken cancellationToken = default);
    }
}