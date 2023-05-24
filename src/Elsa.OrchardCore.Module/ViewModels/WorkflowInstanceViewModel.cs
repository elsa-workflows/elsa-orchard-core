using Elsa.Workflows.Management.Entities;

namespace Elsa.OrchardCore.ViewModels;

public class WorkflowInstanceViewModel
{
    public WorkflowInstance WorkflowInstance { get; set; } = default!;
    public WorkflowDefinition WorkflowDefinition { get; set; } = default!;

}