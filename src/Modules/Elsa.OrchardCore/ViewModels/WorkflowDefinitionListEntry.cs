using Elsa.Models;

namespace Elsa.OrchardCore.ViewModels
{
    public class WorkflowDefinitionListEntry
    {
        public WorkflowDefinitionVersion WorkflowDefinition { get; set; }
        public bool IsChecked { get; set; }
        public int WorkflowInstanceCount { get; set; }
    }
}