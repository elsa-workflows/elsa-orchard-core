using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Elsa.OrchardCore.ViewModels
{
    public class WorkflowDefinitionListOptions
    {
        public string Search { get; set; } = default!;
        public WorkflowDefinitionListBulkAction BulkAction { get; set; }
        public WorkflowDefinitionListOrder Order { get; set; }
        public WorkflowDefinitionListFilter Filter { get; set; }

        [BindNever] public List<SelectListItem> BulkActions { get; set; } = new List<SelectListItem>();
    }
}