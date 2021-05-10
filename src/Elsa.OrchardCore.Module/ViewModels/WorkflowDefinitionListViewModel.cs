using System.Collections.Generic;

namespace Elsa.OrchardCore.ViewModels
{
    public class WorkflowDefinitionListViewModel
    {
        public ICollection<WorkflowDefinitionListEntry> WorkflowDefinitions { get; set; } = new List<WorkflowDefinitionListEntry>();
        public WorkflowDefinitionListOptions Options { get; set; } = new WorkflowDefinitionListOptions();
        public dynamic Pager { get; set; } = default!;
    }
}