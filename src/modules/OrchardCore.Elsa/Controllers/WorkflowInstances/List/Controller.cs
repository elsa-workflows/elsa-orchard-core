using Elsa.Common.Entities;
using Elsa.Common.Models;
using Elsa.Workflows;
using Elsa.Workflows.Management;
using Elsa.Workflows.Management.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using OrchardCore.Admin;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Locking.Distributed;
using OrchardCore.Navigation;
using OrchardCore.Routing;
using YesSql;

namespace OrchardCore.Elsa.Controllers.WorkflowInstances.List;

[Admin]
public sealed class WorkflowInstancesController(
    IWorkflowInstanceStore workflowInstanceStore,
    IOptions<PagerOptions> pagerOptions,
    ISession session,
    IAuthorizationService authorizationService,
    IShapeFactory shapeFactory,
    INotifier notifier,
    IHtmlLocalizer<WorkflowInstancesController> htmlLocalizer,
    IDistributedLock distributedLock,
    IStringLocalizer<WorkflowInstancesController> stringLocalizer,
    IUpdateModelAccessor updateModelAccessor)
    : Controller
{
    private readonly PagerOptions _pagerOptions = pagerOptions.Value;

    internal readonly IHtmlLocalizer H = htmlLocalizer;
    internal readonly IStringLocalizer S = stringLocalizer;

    [Admin("ElsaWorkflows/WorkflowInstances/List")]
    public async Task<IActionResult> List(WorkflowIndexViewModel model, PagerParameters pagerParameters, string? returnUrl = null, CancellationToken cancellationToken = default)
    {
        if (!await authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
            return Forbid();

        if (!Url.IsLocalUrl(returnUrl)) returnUrl = null;

        var filter = new global::Elsa.Workflows.Management.Filters.WorkflowInstanceFilter
        {
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
        var workflowIds = pageOfItems.Items.Select(item => item.Id).ToArray();

        var workflowInstances = pageOfItems.Items;

        var viewModel = new WorkflowIndexViewModel
        {
            Entries = workflowInstances.Select(x => new WorkflowInstanceEntry { WorkflowInstance = x, Id = x.Id }).ToList(),
            Options = model.Options,
            Pager = pagerShape,
            ReturnUrl = returnUrl,
        };

        model.Options.WorkflowsSorts =
        [
            new SelectListItem(S["Recently created"], nameof(WorkflowInstanceOrder.CreatedDesc)),
            new SelectListItem(S["Least recently created"], nameof(WorkflowInstanceOrder.Created)),
        ];

        model.Options.WorkflowsStatuses =
        [
            new SelectListItem(S["All"], nameof(WorkflowInstanceFilter.All)),
            new SelectListItem(S["Faulted"], nameof(WorkflowInstanceFilter.Faulted)),
            new SelectListItem(S["Finished"], nameof(WorkflowInstanceFilter.Finished)),
        ];

        viewModel.Options.WorkflowsBulkAction =
        [
            new SelectListItem(S["Delete"], nameof(WorkflowInstanceBulkAction.Delete)),
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
    
    //
    // [HttpPost]
    // public async Task<IActionResult> Delete(long id)
    // {
    //     if (!await authorizationService.AuthorizeAsync(User, WorkflowsPermissions.ManageWorkflows))
    //     {
    //         return Forbid();
    //     }
    //
    //     var workflow = await workflowInstanceStore.GetAsync(id);
    //
    //     if (workflow == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     var workflowType = await workflowTypeStore.GetAsync(workflow.WorkflowTypeId);
    //     await workflowInstanceStore.DeleteAsync(workflow);
    //     await notifier.SuccessAsync(H["Workflow {0} has been deleted.", id]);
    //     return RedirectToAction(nameof(Index), new { workflowTypeId = workflowType.Id });
    // }
    //
    // [HttpPost]
    // public async Task<IActionResult> Restart(long id)
    // {
    //     if (!await authorizationService.AuthorizeAsync(User, WorkflowsPermissions.ManageWorkflows))
    //     {
    //         return Forbid();
    //     }
    //
    //     var workflow = await workflowInstanceStore.GetAsync(id);
    //
    //     if (workflow == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     var workflowType = await workflowTypeStore.GetAsync(workflow.WorkflowTypeId);
    //
    //     if (workflowType == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     // If a singleton, try to acquire a lock per workflow type.
    //     (var locker, var locked) = await distributedLock.TryAcquireWorkflowTypeLockAsync(workflowType);
    //     if (!locked)
    //     {
    //         await notifier.ErrorAsync(H["Another instance is already running.", id]);
    //     }
    //     else
    //     {
    //         await using var acquiredLock = locker;
    //
    //         // Check if this is a workflow singleton and there's already an halted instance on any activity.
    //         if (workflowType.IsSingleton && await workflowInstanceStore.HasHaltedInstanceAsync(workflowType.WorkflowTypeId))
    //         {
    //             await notifier.ErrorAsync(H["Another instance is already running.", id]);
    //         }
    //         else
    //         {
    //             await workflowManager.RestartWorkflowAsync(workflow, workflowType);
    //
    //             await notifier.SuccessAsync(H["Workflow {0} has been restarted.", id]);
    //         }
    //     }
    //
    //     return RedirectToAction(nameof(Index), new { workflowTypeId = workflowType.Id });
    // }
    //
    // [HttpPost]
    // [ActionName(nameof(Index))]
    // [FormValueRequired("submit.BulkAction")]
    // public async Task<IActionResult> BulkEdit(long workflowTypeId, WorkflowIndexOptions options, PagerParameters pagerParameters, IEnumerable<long> itemIds)
    // {
    //     if (!await authorizationService.AuthorizeAsync(User, WorkflowsPermissions.ManageWorkflows))
    //     {
    //         return Forbid();
    //     }
    //
    //     if (itemIds?.Count() > 0)
    //     {
    //         var checkedEntries = await session.Query<Workflow, WorkflowIndex>().Where(x => x.DocumentId.IsIn(itemIds)).ListAsync();
    //         switch (options.InstanceBulkAction)
    //         {
    //             case WorkflowInstanceBulkAction.None:
    //                 break;
    //             case WorkflowInstanceBulkAction.Delete:
    //                 foreach (var entry in checkedEntries)
    //                 {
    //                     var workflow = await workflowInstanceStore.GetAsync(entry.Id);
    //
    //                     if (workflow != null)
    //                     {
    //                         await workflowInstanceStore.DeleteAsync(workflow);
    //                         await notifier.SuccessAsync(H["Workflow {0} has been deleted.", workflow.Id]);
    //                     }
    //                 }
    //                 break;
    //
    //             default:
    //                 return BadRequest();
    //         }
    //     }
    //     return RedirectToAction(nameof(Index), new { workflowTypeId, pagenum = pagerParameters.Page, pagesize = pagerParameters.PageSize });
    // }
    //
    // private async Task<dynamic> BuildActivityDisplayAsync(ActivityContext activityContext, long workflowTypeId, bool isBlocking, string displayType)
    // {
    //     var activityShape = await activityDisplayManager.BuildDisplayAsync(activityContext.Activity, updateModelAccessor.ModelUpdater, displayType);
    //     activityShape.Metadata.Type = $"Activity_{displayType}ReadOnly";
    //     activityShape.Properties["Activity"] = activityContext.Activity;
    //     activityShape.Properties["ActivityRecord"] = activityContext.ActivityRecord;
    //     activityShape.Properties["WorkflowTypeId"] = workflowTypeId;
    //     activityShape.Properties["IsBlocking"] = isBlocking;
    //
    //     return activityShape;
    // }
}
