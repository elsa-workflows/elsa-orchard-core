using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Common.Models;
using Elsa.OrchardCore.Contracts;
using Elsa.OrchardCore.ViewModels;
using Elsa.Workflows.Management.Contracts;
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
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Environment.Shell;
using OrchardCore.Navigation;
using OrchardCore.Routing;

namespace Elsa.OrchardCore.Controllers;

[Admin]
public class WorkflowInstanceController : Controller
{
    private readonly PagerOptions _pagerOptions;
    private readonly IWorkflowDefinitionService _workflowDefinitionService;
    private readonly IWorkflowInstanceStore _workflowInstanceStore;
    private readonly IElsaServerUrlAccessor _elsaServerUrlAccessor;
    private readonly IAuthorizationService _authorizationService;
    private readonly INotifier _notifier;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private readonly IHtmlLocalizer H;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private readonly IStringLocalizer S;

    public WorkflowInstanceController(
        IWorkflowDefinitionService workflowDefinitionService,
        IWorkflowInstanceStore workflowInstanceStore,
        IElsaServerUrlAccessor elsaServerUrlAccessor,
        IOptions<PagerOptions> pagerOptions,
        IAuthorizationService authorizationService,
        IShapeFactory shapeFactory,
        INotifier notifier,
        IHtmlLocalizer<WorkflowInstanceController> htmlLocalizer,
        IStringLocalizer<WorkflowInstanceController> stringLocalizer)
    {
        _pagerOptions = pagerOptions.Value;
        _workflowDefinitionService = workflowDefinitionService;
        _workflowInstanceStore = workflowInstanceStore;
        _elsaServerUrlAccessor = elsaServerUrlAccessor;
        _authorizationService = authorizationService;
        _notifier = notifier;
        New = shapeFactory;
        H = htmlLocalizer;
        S = stringLocalizer;
    }

    private dynamic New { get; }

    public async Task<IActionResult> Index(string definitionId, WorkflowInstanceIndexViewModel model, PagerParameters pagerParameters, string? returnUrl, CancellationToken cancellationToken)
    {
        if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
            return Forbid();

        if (!Url.IsLocalUrl(returnUrl))
            returnUrl = Url.Action(nameof(Index), "WorkflowDefinition");

        var workflowDefinition = await _workflowDefinitionService.FindAsync(definitionId, VersionOptions.Latest, cancellationToken);

        if (workflowDefinition == null)
            return NotFound();

        // switch (model.Options.Filter)
        // {
        //     case WorkflowFilter.Finished:
        //         query = query.Where(x => x.WorkflowStatus == (int)WorkflowStatus.Finished);
        //         break;
        //     case WorkflowFilter.Faulted:
        //         query = query.Where(x => x.WorkflowStatus == (int)WorkflowStatus.Faulted);
        //         break;
        //     case WorkflowFilter.All:
        //     default:
        //         break;
        // }
        //
        // switch (model.Options.OrderBy)
        // {
        //     case WorkflowOrder.CreatedDesc:
        //         query = query.OrderByDescending(x => x.CreatedUtc);
        //         break;
        //     case WorkflowOrder.Created:
        //         query = query.OrderBy(x => x.CreatedUtc);
        //         break;
        //     default:
        //         query = query.OrderByDescending(x => x.CreatedUtc);
        //         break;
        // }

        var pager = new Pager(pagerParameters, _pagerOptions.GetPageSize());
        var routeData = new RouteData();
        routeData.Values.Add("Filter", model.Options.Filter);

        var workflowInstanceFilter = new WorkflowInstanceFilter
        {
            DefinitionId = definitionId,
        };

        var pageArgs = new PageArgs(pager.Page - 1, pager.PageSize);
        var workflowInstances = await _workflowInstanceStore.FindManyAsync(workflowInstanceFilter, pageArgs, cancellationToken);
        var totalCount = workflowInstances.TotalCount;

        var pagerShape = (await New.Pager(pager)).TotalItemCount(totalCount).RouteData(routeData);
        var pageOfItems = workflowInstances.Items;

        var viewModel = new WorkflowInstanceIndexViewModel
        {
            WorkflowDefinition = workflowDefinition,
            WorkflowInstances = pageOfItems.Select(x => new WorkflowInstanceEntry
            {
                WorkflowInstance = x,
                Id = x.Id
            }).ToList(),
            Options = model.Options,
            Pager = pagerShape,
            ReturnUrl = returnUrl
        };

        model.Options.WorkflowsSorts = new List<SelectListItem>()
        {
            new() { Text = S["Recently created"], Value = nameof(WorkflowInstanceOrder.CreatedDesc) },
            new() { Text = S["Least recently created"], Value = nameof(WorkflowInstanceOrder.Created) }
        };

        model.Options.WorkflowsStatuses = new List<SelectListItem>()
        {
            new() { Text = S["All"], Value = nameof(WorkflowInstanceStatusFilter.All) },
            new() { Text = S["Faulted"], Value = nameof(WorkflowInstanceStatusFilter.Faulted) },
            new() { Text = S["Finished"], Value = nameof(WorkflowInstanceStatusFilter.Finished) }
        };

        viewModel.Options.WorkflowsBulkAction = new List<SelectListItem>()
        {
            new() { Text = S["Delete"], Value = nameof(WorkflowInstanceBulkAction.Delete) }
        };

        return View(viewModel);
    }

    [HttpPost, ActionName("Index")]
    [FormValueRequired("submit.Filter")]
    public ActionResult IndexFilterPost(WorkflowInstanceIndexViewModel model)
    {
        return RedirectToAction(nameof(Index), new RouteValueDictionary
        {
            { "Options.Filter", model.Options.Filter },
            { "Options.OrderBy", model.Options.OrderBy }
        });
    }

    public async Task<IActionResult> Details(string id, CancellationToken cancellationToken)
    {
        if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
            return Forbid();

        var workflowInstance = await _workflowInstanceStore.FindAsync(new WorkflowInstanceFilter { Id = id }, cancellationToken);

        if (workflowInstance == null)
            return NotFound();

        var workflowDefinition = await _workflowDefinitionService.FindAsync(workflowInstance.DefinitionId, VersionOptions.SpecificVersion(workflowInstance.Version), cancellationToken);
        
        if(workflowDefinition == null)
            return NotFound();

        var serverUrl = await _elsaServerUrlAccessor.GetServerUrlAsync(cancellationToken);
        var viewModel = new WorkflowInstanceViewModel(workflowDefinition, workflowInstance, serverUrl, workflowInstance.Id);
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
            return Forbid();

        var workflowInstance = await _workflowInstanceStore.FindAsync(new WorkflowInstanceFilter { Id = id }, cancellationToken);

        if (workflowInstance == null)
            return NotFound();
        
        await _workflowInstanceStore.DeleteAsync(new WorkflowInstanceFilter{ Id = workflowInstance.Id}, cancellationToken);
        await _notifier.SuccessAsync(H["Workflow {0} has been deleted.", id]);
        return RedirectToAction(nameof(Index), new { workflowDefinitionId = workflowInstance.DefinitionId });
    }

    [HttpPost]
    [ActionName(nameof(Index))]
    [FormValueRequired("submit.BulkAction")]
    public async Task<IActionResult> BulkEdit(int workflowTypeId, WorkflowInstanceIndexOptions options, PagerParameters pagerParameters, IEnumerable<string>? itemIds, CancellationToken cancellationToken)
    {
        if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
            return Forbid();

        var workflowInstanceIds = itemIds?.ToList() ?? new List<string>();
        
        if (workflowInstanceIds.Any())
        {
            switch (options.BulkAction)
            {
                case WorkflowInstanceBulkAction.None:
                    break;
                case WorkflowInstanceBulkAction.Delete:
                    var count = await _workflowInstanceStore.DeleteAsync(new WorkflowInstanceFilter { Ids = workflowInstanceIds }, cancellationToken);
                    
                    if(count == 1)
                        await _notifier.SuccessAsync(H["1 workflow has been deleted."]);
                    else
                        await _notifier.SuccessAsync(H["{0} workflows have been deleted.", count]);
                    
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return RedirectToAction(nameof(Index), new { workflowTypeId, pagenum = pagerParameters.Page, pagesize = pagerParameters.PageSize });
    }
}