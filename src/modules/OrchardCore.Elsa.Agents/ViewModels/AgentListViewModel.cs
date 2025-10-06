using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OrchardCore.Elsa.Agents.ViewModels;

public class AgentListViewModel
{
    public IList<AgentSummaryViewModel> Items { get; set; } = [];
    public AgentListOptions Options { get; set; } = new();
    public dynamic? Pager { get; set; }
}

public class AgentSummaryViewModel
{
    public string AgentId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public bool IsLatest { get; set; }
}

public class AgentListOptions
{
    public string Search { get; set; } = string.Empty;
    public AgentFilter Filter { get; set; }
    public AgentBulkAction BulkAction { get; set; }

    [BindNever]
    public IList<SelectListItem> BulkActions { get; set; } = [];
}

public enum AgentFilter
{
    All,
    Published,
    Draft
}

public enum AgentBulkAction
{
    None,
    Publish,
    Unpublish,
    Delete
}
