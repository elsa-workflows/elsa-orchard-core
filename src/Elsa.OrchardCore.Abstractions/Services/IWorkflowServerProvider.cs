using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.OrchardCore.Models;

namespace Elsa.OrchardCore.Services
{
    public interface IWorkflowServerProvider
    {
        Task<IEnumerable<WorkflowServer>> ListWorkflowServersAsync(CancellationToken cancellationToken = default);
    }
}