namespace Elsa.OrchardCore.Contracts;

public interface IWorkflowServerClient
{
    IWorkflowServerDefinitionsApiClient WorkflowDefinitions { get; }
}