using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using OrchardCore.ContentManagement;

namespace OrchardCore.Elsa.Controllers.WorkflowDefinitions.List;

public class ListViewModel
{
    public IList<ListItemViewModel> Items { get; set; } = [];
    public ListOptions Options { get; set; } = null!;
    public dynamic Pager { get; set; } = null!;
}

public class ListItemViewModel
{
    public long Id { get; set; }
    public bool IsChecked { get; set; }
    public string Name { get; set; } = null!;
    public ContentItem WorkflowDefinition { get; set; } = null!;
    public dynamic ContentItemSummary { get; set; } = null!;
}

public class ListOptions
{
    public string Search { get; set; } = string.Empty;
    public BulkAction BulkAction { get; set; }
    public Order Order { get; set; }
    public Filter Filter { get; set; }

    [BindNever] public IList<SelectListItem> WorkflowTypesBulkAction { get; set; } = [];
}

public enum Order
{
    Name,
    Creation
}

public enum Filter
{
    All
}

public enum BulkAction
{
    None,
    Export,
    Delete
}
