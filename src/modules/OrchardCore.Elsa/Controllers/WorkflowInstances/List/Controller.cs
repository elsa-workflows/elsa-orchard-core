using Elsa.Common.Entities;
using Elsa.Common.Models;
using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Management;
using Elsa.Workflows.Management.Filters;
using Elsa.Workflows.Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using OrchardCore.Admin;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Navigation;
using OrchardCore.Routing;

namespace OrchardCore.Elsa.Controllers.WorkflowInstances.List;

[Admin]
public sealed class WorkflowInstancesController(
    IWorkflowInstanceManager workflowInstanceManager,
    IWorkflowInstanceStore workflowInstanceStore,
    IWorkflowDefinitionStore workflowDefinitionStore,
    IOptions<PagerOptions> pagerOptions,
    IAuthorizationService authorizationService,
    IShapeFactory shapeFactory,
    INotifier notifier,
    IHtmlLocalizer<WorkflowInstancesController> htmlLocalizer,
    IStringLocalizer<WorkflowInstancesController> stringLocalizer)
    : Controller
{
    private readonly PagerOptions _pagerOptions = pagerOptions.Value;

    private IHtmlLocalizer H => htmlLocalizer;
    private IStringLocalizer S => stringLocalizer;

    [Admin("ElsaWorkflows/WorkflowInstances/List")]
    public async Task<IActionResult> List(WorkflowIndexViewModel model, PagerParameters pagerParameters, string? returnUrl = null, CancellationToken cancellationToken = default)
    {
        if (!await authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
            return Forbid();

        if (!Url.IsLocalUrl(returnUrl)) returnUrl = null;

        var filter = new global::Elsa.Workflows.Management.Filters.WorkflowInstanceFilter
        {
            DefinitionId = model.Options.SelectedWorkflowDefinitionId,
            WorkflowSubStatus = model.Options.InstanceFilter switch
            {
                WorkflowInstanceFilter.Finished => WorkflowSubStatus.Finished,
                WorkflowInstanceFilter.Faulted => WorkflowSubStatus.Faulted,
                _ => null
            }
        };

        WorkflowInstanceOrder<DateTimeOffset> orderBy = model.Options.InstanceOrderBy switch
        {
            WorkflowInstanceOrder.Created => new(x => x.CreatedAt, OrderDirection.Ascending),
            _ => new(x => x.CreatedAt, OrderDirection.Descending),
        };

        var pageArgs = PageArgs.FromPage(pagerParameters.Page, pagerParameters.PageSize);
        var pageOfItems = await workflowInstanceStore.SummarizeManyAsync(filter, pageArgs, orderBy, cancellationToken);
        var pager = new Pager(pagerParameters, _pagerOptions.GetPageSize());
        var routeData = new RouteData();
        routeData.Values.Add("Filter", model.Options.InstanceFilter);

        var pagerShape = await shapeFactory.PagerAsync(pager, (int)pageOfItems.TotalCount, routeData);
        var workflowInstances = pageOfItems.Items;

        var viewModel = new WorkflowIndexViewModel
        {
            WorkflowDefinitions = (await GetWorkflowDefinitionSummariesAsync(cancellationToken)).ToDictionary(x => x.DefinitionId),
            Entries = workflowInstances.Select(x => new WorkflowInstanceEntry { WorkflowInstance = x, Id = x.Id }).ToList(),
            Options = model.Options,
            Pager = pagerShape,
            ReturnUrl = returnUrl,
        };

        model.Options.WorkflowsSorts =
        [
            new(S["Recently created"], nameof(WorkflowInstanceOrder.CreatedDesc)),
            new(S["Least recently created"], nameof(WorkflowInstanceOrder.Created)),
        ];

        model.Options.WorkflowsStatuses =
        [
            new(S["All"], nameof(WorkflowInstanceFilter.All)),
            new(S["Faulted"], nameof(WorkflowInstanceFilter.Faulted)),
            new(S["Finished"], nameof(WorkflowInstanceFilter.Finished)),
        ];

        viewModel.Options.WorkflowsBulkAction =
        [
            new(S["Delete"], nameof(WorkflowInstanceBulkAction.Delete)),
        ];

        return View(viewModel);
    }

    [HttpPost, ActionName(nameof(Index))]
    [FormValueRequired("submit.Filter")]
    public ActionResult ListFilterPost(WorkflowIndexViewModel model)
    {
        return RedirectToAction(nameof(List), new RouteValueDictionary
        {
            { "Options.Filter", model.Options.InstanceFilter },
            { "Options.OrderBy", model.Options.InstanceOrderBy },
        });
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken = default)
    {
        if (!await authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
            return Forbid();
        
        var filter = new global::Elsa.Workflows.Management.Filters.WorkflowInstanceFilter
        {
            Id = id
        };
        var found = await workflowInstanceManager.DeleteAsync(filter, cancellationToken);
        
        if (!found)
            return NotFound();
        
        await notifier.SuccessAsync(H["Workflow Instance {0} has been deleted.", id]);
        return RedirectToAction(nameof(List));
    }
    
    [HttpPost]
    [ActionName(nameof(List))]
    [FormValueRequired("submit.BulkAction")]
    public async Task<IActionResult> BulkEdit(WorkflowIndexOptions options, PagerParameters pagerParameters, string[] itemIds)
    {
        if (!await authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
            return Forbid();

        if (!itemIds.Any()) 
            return RedirectToAction(nameof(List), new { page = pagerParameters.Page, pageSize = pagerParameters.PageSize });
        
        switch (options.InstanceBulkAction)
        {
            case WorkflowInstanceBulkAction.None:
                break;
            case WorkflowInstanceBulkAction.Delete:
                    
                var filter = new global::Elsa.Workflows.Management.Filters.WorkflowInstanceFilter
                {
                    Ids = itemIds
                };
                var count = await workflowInstanceManager.BulkDeleteAsync(filter);
                    
                await notifier.SuccessAsync(H["{0} workflow instances have been deleted.", count]);
                break;

            default:
                return BadRequest();
        }
        
        return RedirectToAction(nameof(List), new { page = pagerParameters.Page, pageSize = pagerParameters.PageSize });
    }
    
    private async Task<IList<WorkflowDefinitionSummary>> GetWorkflowDefinitionSummariesAsync(CancellationToken cancellationToken = default)
    {
        var filter = new WorkflowDefinitionFilter
        {
            VersionOptions = VersionOptions.LatestOrPublished
        };
        var summaries = await workflowDefinitionStore.FindSummariesAsync(filter, cancellationToken);

        // Filter the definitions to ensure only the one with the highest version for each DefinitionId remains
        var filteredSummaries = summaries
            .GroupBy(definition => definition.DefinitionId) // Group by DefinitionId
            .Select(group => group.OrderByDescending(definition => definition.Version).First()) // Get the highest version in each group
            .ToList();

        return filteredSummaries;
    }
}
