using System;
using System.Threading;
using System.Threading.Tasks;

namespace Elsa.OrchardCore.Services
{
    public interface IWorkflowServer
    {
        string Id { get; }
        string Name { get; }
        IWorkflowServerClient CreateClient();
        ValueTask<Uri> GetServerUrlAsync(CancellationToken cancellationToken = default);
    }
}