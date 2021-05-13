using System.Threading;
using System.Threading.Tasks;

namespace Elsa.OrchardCore.Services
{
    public interface IWorkflowServerClientFactory
    {
        Task<IWorkflowServerClient> CreateClientAsync(string serverId, CancellationToken cancellationToken = default);
        Task<IWorkflowServerClient> CreateClientAsync(IWorkflowServer server, CancellationToken cancellationToken = default);
    }
}