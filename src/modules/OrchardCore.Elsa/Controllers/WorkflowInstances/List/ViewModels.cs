using Elsa.Workflows.Management.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OrchardCore.Elsa.Controllers.WorkflowInstances.List;

public class WorkflowIndexViewModel
{
    public IDictionary<string, WorkflowDefinitionSummary> WorkflowDefinitions { get; set; } = new Dictionary<string, WorkflowDefinitionSummary>();
    
    public ICollection<SelectListItem> WorkflowDefinitionItems => WorkflowDefinitions
        .Select(x => new SelectListItem(x.Value.Name, x.Key))
        .Prepend(new() { Text = "All", Value = string.Empty })
        .ToList();
    
    public IList<WorkflowInstanceEntry> Entries { get; set; } = [];
    public WorkflowIndexOptions Options { get; set; } = new();
    public dynamic Pager { get; set; } = null!;
    public string? ReturnUrl { get; set; }
}

public class WorkflowIndexOptions
{
    public string? SelectedWorkflowDefinitionId { get; set; }
    public WorkflowInstanceBulkAction InstanceBulkAction { get; set; }
    public WorkflowInstanceFilter InstanceFilter { get; set; } = WorkflowInstanceFilter.All;

    public WorkflowInstanceOrder InstanceOrderBy { get; set; }

    [BindNever] public List<SelectListItem> WorkflowsSorts { get; set; } = [];

    [BindNever] public List<SelectListItem> WorkflowsStatuses { get; set; } = [];

    [BindNever] public List<SelectListItem> WorkflowsBulkAction { get; set; } = [];
}

public class WorkflowInstanceEntry
{
    public WorkflowInstanceSummary WorkflowInstance { get; set; } = null!;
    public string Id { get; set; } = null!;
    public bool IsChecked { get; set; }
}

public enum WorkflowInstanceFilter
{
    All,
    Finished,
    Faulted
}

public enum WorkflowInstanceOrder
{
    CreatedDesc,
    Created
}

public enum WorkflowInstanceBulkAction
{
    None,
    Delete
}