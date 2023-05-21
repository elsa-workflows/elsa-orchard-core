using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.OrchardCore.Models;

namespace Elsa.OrchardCore.Contracts
{
    public interface IRemoteWorkflowServerStore
    {
        Task<IEnumerable<RemoteWorkflowServerRecord>> ListAsync(CancellationToken cancellationToken = default);
        Task<RemoteWorkflowServerRecord> SaveAsync(RemoteWorkflowServerRecord remoteWorkflowServerRecord, CancellationToken cancellationToken = default);
        Task<RemoteWorkflowServerRecord?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task DeleteAsync(RemoteWorkflowServerRecord remoteWorkflowServerRecord, CancellationToken cancellationToken = default);
    }
}