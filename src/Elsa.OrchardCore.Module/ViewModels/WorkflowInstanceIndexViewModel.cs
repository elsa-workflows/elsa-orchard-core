using System.Collections.Generic;
using Elsa.Workflows.Management.Entities;
using Elsa.Workflows.Management.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Elsa.OrchardCore.ViewModels;

public class WorkflowInstanceIndexViewModel
{
    public WorkflowInstanceIndexViewModel()
    {
        Options = new WorkflowInstanceIndexOptions();
    }

    public WorkflowDefinition WorkflowDefinition { get; set; } = default!;
    public IList<WorkflowInstanceEntry> WorkflowInstances { get; set; } = default!;
    public WorkflowInstanceIndexOptions Options { get; set; }
    public dynamic Pager { get; set; } = default!;
    public string? ReturnUrl { get; set; }
}

public class WorkflowInstanceIndexOptions
{
    public WorkflowInstanceIndexOptions()
    {
        Filter = WorkflowInstanceStatusFilter.All;
    }

    public WorkflowInstanceBulkAction BulkAction { get; set; }
    public WorkflowInstanceStatusFilter Filter { get; set; }

    public WorkflowInstanceOrder OrderBy { get; set; }

    [BindNever] public List<SelectListItem> WorkflowsSorts { get; set; } = default!;

    [BindNever] public List<SelectListItem> WorkflowsStatuses { get; set; } = default!;

    [BindNever] public List<SelectListItem> WorkflowsBulkAction { get; set; } = default!;
}

public class WorkflowInstanceEntry
{
    public WorkflowInstance WorkflowInstance { get; set; } = default!;
    public string Id { get; set; } = default!;
    public bool IsChecked { get; set; }
}

public enum WorkflowInstanceStatusFilter
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