using Elsa.OrchardCore.Contracts;

namespace Elsa.OrchardCore.Features.LocalWorkflowServer.Services;

public class LocalWorkflowServerClient : IWorkflowServerClient
{
    public LocalWorkflowServerClient(IWorkflowServerDefinitionsApiClient workflowDefinitions)
    {
        WorkflowDefinitions = workflowDefinitions;
    }

    public IWorkflowServerDefinitionsApiClient WorkflowDefinitions { get; }
}