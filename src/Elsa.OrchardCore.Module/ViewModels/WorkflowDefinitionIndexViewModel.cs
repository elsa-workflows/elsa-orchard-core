using System.Collections.Generic;

namespace Elsa.OrchardCore.ViewModels
{
    public record WorkflowDefinitionIndexViewModel
    {
        public string ServerId { get; set; } = default!;
        public IList<WorkflowDefinitionListEntry> WorkflowDefinitions { get; set; } = new List<WorkflowDefinitionListEntry>();
        public WorkflowDefinitionIndexOptions Options { get; set; } = new();
        public dynamic Pager { get; set; } = default!;
    }
}