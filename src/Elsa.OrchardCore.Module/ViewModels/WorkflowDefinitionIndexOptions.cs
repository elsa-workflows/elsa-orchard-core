using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Elsa.OrchardCore.ViewModels
{
    public class WorkflowDefinitionIndexOptions
    {
        public string? Search { get; set; }
        public WorkflowDefinitionBulkAction BulkAction { get; set; }
        public WorkflowDefinitionOrder Order { get; set; }
        public WorkflowDefinitionFilter Filter { get; set; }
        [BindNever] public List<SelectListItem> WorkflowTypesBulkAction { get; set; } = new();
    }
}