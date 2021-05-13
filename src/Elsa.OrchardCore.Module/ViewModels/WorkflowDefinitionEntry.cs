using Elsa.Client.Models;

namespace Elsa.OrchardCore.ViewModels
{
    public record WorkflowDefinitionEntry
    (
        WorkflowDefinitionSummary WorkflowDefinition,
        bool IsChecked,
        string Id,
        string Name,
        int WorkflowCount
    );
}