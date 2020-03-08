using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.OrchardCore.Indexes;
using Elsa.OrchardCore.Models;
using YesSql;

namespace Elsa.OrchardCore.Services
{
    public class WorkflowServerStore : IWorkflowServerStore
    {
        private readonly ISession _session;
        public WorkflowServerStore(ISession session) => _session = session;

        public Task<IEnumerable<WorkflowServer>> ListAsync(CancellationToken cancellationToken) => _session.Query<WorkflowServer>().ListAsync();
        
        public Task<WorkflowServer> SaveAsync(WorkflowServer workflowServer, CancellationToken cancellationToken)
        {
             _session.Save(workflowServer);
             return Task.FromResult(workflowServer);
        }

        public Task<WorkflowServer?> GetByIdAsync(string id, in CancellationToken cancellationToken = default) => 
            _session.Query<WorkflowServer, WorkflowServerIndex>(x => x.WorkflowServerId == id).FirstOrDefaultAsync();

        public Task DeleteAsync(WorkflowServer workflowServer, CancellationToken cancellationToken)
        {
            _session.Delete(workflowServer);
            return Task.CompletedTask;
        }
    }
}