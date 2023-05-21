using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.OrchardCore.Contracts;
using Elsa.OrchardCore.Indexes;
using Elsa.OrchardCore.Models;
using YesSql;

namespace Elsa.OrchardCore.Features.RemoteWorkflows.Services;

public class RemoteWorkflowServerStore : IRemoteWorkflowServerStore
{
    private readonly ISession _session;
    public RemoteWorkflowServerStore(ISession session) => _session = session;

    public Task<IEnumerable<RemoteWorkflowServerRecord>> ListAsync(CancellationToken cancellationToken) => _session.Query<RemoteWorkflowServerRecord>().ListAsync();

    public Task<RemoteWorkflowServerRecord> SaveAsync(RemoteWorkflowServerRecord remoteWorkflowServer, CancellationToken cancellationToken)
    {
        _session.Save(remoteWorkflowServer);
        return Task.FromResult(remoteWorkflowServer);
    }

    public async Task<RemoteWorkflowServerRecord?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        await _session.Query<RemoteWorkflowServerRecord, WorkflowServerRecordIndex>(x => x.WorkflowServerId == id).FirstOrDefaultAsync();

    public Task DeleteAsync(RemoteWorkflowServerRecord remoteWorkflowServer, CancellationToken cancellationToken)
    {
        _session.Delete(remoteWorkflowServer);
        return Task.CompletedTask;
    }
}