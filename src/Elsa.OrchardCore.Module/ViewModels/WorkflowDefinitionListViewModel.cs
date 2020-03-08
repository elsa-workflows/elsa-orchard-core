using System.Collections.Generic;

namespace Elsa.OrchardCore.ViewModels
{
    public class WorkflowDefinitionListViewModel
    {
        public ICollection<WorkflowDefinitionListEntry> WorkflowDefinitions { get; set; }
        public WorkflowDefinitionListOptions Options { get; set; }
        public dynamic Pager { get; set; }
    }
}