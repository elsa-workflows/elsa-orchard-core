using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Elsa.OrchardCore.Contracts
{
    public interface IWorkflowServerProvider
    {
        ValueTask<IEnumerable<IWorkflowServer>> ListWorkflowServersAsync(CancellationToken cancellationToken = default);
    }
}