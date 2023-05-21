using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elsa.Api.Client.Contracts;
using Elsa.Api.Client.Resources.WorkflowDefinitions.Models;
using Elsa.OrchardCore.Contracts;
using Elsa.OrchardCore.ViewModels;
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
using OrchardCore.Navigation;
using OrchardCore.Routing;

namespace Elsa.OrchardCore.Controllers
{
    [Admin]
    public class WorkflowDefinitionController : Controller
    {
        private readonly PagerOptions _pagerOptions;
        private readonly IElsaClient _elsaClient;
        private readonly IWorkflowServerManager _workflowServerManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly INotifier _notifier;

        private readonly dynamic New;
        private readonly IStringLocalizer S;
        private readonly IHtmlLocalizer H;

        public WorkflowDefinitionController
        (
            IElsaClient elsaClient,
            IOptions<PagerOptions> pagerOptions,
            IAuthorizationService authorizationService,
            IShapeFactory shapeFactory,
            INotifier notifier,
            IStringLocalizer<WorkflowDefinitionController> s,
            IHtmlLocalizer<WorkflowDefinitionController> h, IWorkflowServerManager workflowServerManager)
        {
            _pagerOptions = pagerOptions.Value;
            _elsaClient = elsaClient;
            _authorizationService = authorizationService;
            _notifier = notifier;

            New = shapeFactory;
            S = s;
            H = h;
            _workflowServerManager = workflowServerManager;
        }

        
        public async Task<IActionResult> Index(WorkflowDefinitionIndexOptions? options, PagerParameters pagerParameters)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
            {
                return Forbid();
            }

            var pager = new Pager(pagerParameters, _pagerOptions.GetPageSize());

            options ??= new WorkflowDefinitionIndexOptions();

            //var query = _session.Query<WorkflowType, WorkflowTypeIndex>();

            // switch (options.Filter)
            // {
            //     case WorkflowTypeFilter.All:
            //     default:
            //         break;
            // }

            // if (!string.IsNullOrWhiteSpace(options.Search))
            // {
            //     query = query.Where(x => x.Name.Contains(options.Search));
            // }

            // switch (options.Order)
            // {
            //     case WorkflowTypeOrder.Name:
            //         query = query.OrderBy(u => u.Name);
            //         break;
            // }

            // var count = await query.CountAsync();
            //
            // var workflowTypes = await query
            //     .Skip(pager.GetStartIndex())
            //     .Take(pager.PageSize)
            //     .ListAsync();
            //
            // var workflowTypeIds = workflowTypes.Select(x => x.WorkflowTypeId).ToList();
            // var workflowGroups = (await _session.QueryIndex<WorkflowIndex>(x => x.WorkflowTypeId.IsIn(workflowTypeIds))
            //     .ListAsync())
            //     .GroupBy(x => x.WorkflowTypeId)
            //     .ToDictionary(x => x.Key);

            var request = new ListWorkflowDefinitionsRequest
            {
                Page = pager.Page,
                PageSize = pager.PageSize
            };
            
            var response = await _elsaClient.WorkflowDefinitions.ListAsync(request);
            var workflowDefinitions = response.Items;
            var count = response.TotalCount;
            
            // Maintain previous route data when generating page links.
            var routeData = new RouteData();
            routeData.Values.Add("Options.Filter", options.Filter);
            routeData.Values.Add("Options.Search", options.Search);
            routeData.Values.Add("Options.Order", options.Order);

            var pagerShape = (await New.Pager(pager)).TotalItemCount(count).RouteData(routeData);
            
            var model = new WorkflowDefinitionIndexViewModel
            {
                WorkflowDefinitions = workflowDefinitions
                    .Select(x => new WorkflowDefinitionEntry
                    {
                        WorkflowDefinitionSummary = x,
                        Id = x.Id,
                        WorkflowInstanceCount = 0,
                        Name = x.Name
                    })
                    .ToList(),
                Options = options,
                Pager = pagerShape
            };

            model.Options.WorkflowDefinitionsBulkAction = new List<SelectListItem>() {
                new() { Text = S["Delete"].Value, Value = nameof(WorkflowDefinitionBulkAction.Delete) }
            };

            return View(model);
        }

        [HttpPost, ActionName("Index")]
        [FormValueRequired("submit.Filter")]
        public ActionResult IndexFilterPOST(WorkflowDefinitionIndexViewModel model)
        {
            return RedirectToAction(nameof(Index), new RouteValueDictionary {
                { "Options.Search", model.Options.Search }
            });
        }

        [HttpPost]
        [ActionName(nameof(Index))]
        [FormValueRequired("submit.BulkAction")]
        public async Task<IActionResult> BulkEdit(WorkflowDefinitionIndexOptions options, IEnumerable<int>? ids)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
                return Forbid();

            var itemIds = ids?.ToArray() ?? Array.Empty<int>();

            if (!itemIds.Any()) 
                return RedirectToAction(nameof(Index));
            
            // var checkedEntries = await _session.Query<WorkflowType, WorkflowTypeIndex>().Where(x => x.DocumentId.IsIn(itemIds)).ListAsync();
            // switch (options.BulkAction)
            // {
            //     case WorkflowTypeBulkAction.None:
            //         break;
            //     case WorkflowTypeBulkAction.Delete:
            //         foreach (var entry in checkedEntries)
            //         {
            //             var workflowType = await _workflowTypeStore.GetAsync(entry.Id);
            //
            //             if (workflowType != null)
            //             {
            //                 await _workflowTypeStore.DeleteAsync(workflowType);
            //                 await _notifier.SuccessAsync(H["Workflow {0} has been deleted.", workflowType.Name]);
            //             }
            //         }
            //         break;
            //
            //     default:
            //         throw new ArgumentOutOfRangeException();
            // }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
            {
                return Forbid();
            }

            //await _workflowTypeStore.DeleteAsync(workflowDefinition);
            //await _notifier.SuccessAsync(H["Workflow {0} deleted", workflowDefinition.Name]);

            return RedirectToAction(nameof(Index));
        }
    }
}
