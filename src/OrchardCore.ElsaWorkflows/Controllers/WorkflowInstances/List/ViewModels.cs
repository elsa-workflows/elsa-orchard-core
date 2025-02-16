using System.Collections.Generic;
using Elsa.Workflows.Management.Entities;
using Elsa.Workflows.Management.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using OrchardCore.Workflows.Models;

namespace OrchardCore.ElsaWorkflows.Controllers.WorkflowInstances.List;

public class WorkflowIndexViewModel
{
    public IList<WorkflowInstanceEntry> Entries { get; set; } = [];
    public WorkflowIndexOptions Options { get; set; } = new();
    public dynamic Pager { get; set; } = null!;
    public string? ReturnUrl { get; set; }
}

public class WorkflowIndexOptions
{
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