using Elsa.OrchardCore.Contracts;

namespace Elsa.OrchardCore.Features.RemoteWorkflows.Services;

public class RemoteWorkflowServerClient : IWorkflowServerClient
{
    public RemoteWorkflowServerClient(IWorkflowServerDefinitionsApiClient workflowDefinitions)
    {
        WorkflowDefinitions = workflowDefinitions;
    }

    public IWorkflowServerDefinitionsApiClient WorkflowDefinitions { get; }
}