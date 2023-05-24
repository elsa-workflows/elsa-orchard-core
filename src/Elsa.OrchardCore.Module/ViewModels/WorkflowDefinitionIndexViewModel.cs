using System.Collections.Generic;
using Elsa.Workflows.Management.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Elsa.OrchardCore.ViewModels;

public class WorkflowDefinitionIndexViewModel
{
    public IList<WorkflowDefinitionEntry> WorkflowDefinitions { get; set; } = default!;
    public WorkflowDefinitionIndexOptions Options { get; set; } = default!;
    public dynamic Pager { get; set; } = default!;
}

public class WorkflowDefinitionEntry
{
    public WorkflowDefinitionSummary WorkflowDefinitionSummary { get; set; } = default!;
    public bool IsChecked { get; set; }
    public string Id { get; set; } = default!;
    public string? Name { get; set; }
    public int WorkflowInstanceCount { get; set; }
}

public class WorkflowDefinitionIndexOptions
{
    public string? Search { get; set; }
    public WorkflowDefinitionBulkAction BulkAction { get; set; }
    public WorkflowDefinitionOrder Order { get; set; }
    public WorkflowDefinitionFilter Filter { get; set; }

    [BindNever] public List<SelectListItem> WorkflowDefinitionsBulkAction { get; set; } = default!;
}

public enum WorkflowDefinitionOrder
{
    Name,
    Creation
}

public enum WorkflowDefinitionFilter
{
    All
}

public enum WorkflowDefinitionBulkAction
{
    None,
    Delete
}