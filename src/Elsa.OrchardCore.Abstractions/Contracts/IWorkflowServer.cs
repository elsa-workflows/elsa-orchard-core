using System;
using System.Threading;
using System.Threading.Tasks;

namespace Elsa.OrchardCore.Contracts
{
    /// <summary>
    /// Represents a workflow server.
    /// </summary>
    public interface IWorkflowServer
    {
        string Id { get; }
        string Name { get; }
        IWorkflowServerClient CreateClient();
        ValueTask<Uri> GetServerUrlAsync(CancellationToken cancellationToken = default);
    }
}