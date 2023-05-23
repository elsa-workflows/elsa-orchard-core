using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Common.Models;
using Elsa.OrchardCore.ViewModels;
using Elsa.Workflows.Management.Contracts;
using Elsa.Workflows.Management.Mappers;
using Medallion.Threading;
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
using WorkflowDefinitionFilter = Elsa.Workflows.Management.Filters.WorkflowDefinitionFilter;

namespace Elsa.OrchardCore.Controllers
{
    [Admin]
    public class WorkflowDefinitionController : Controller
    {
        private readonly PagerOptions _pagerOptions;
        private readonly IWorkflowDefinitionStore _workflowDefinitionStore;
        private readonly IDistributedLockProvider _distributedLockProvider;
        private readonly IWorkflowDefinitionPublisher _workflowDefinitionPublisher;
        private readonly WorkflowDefinitionMapper _workflowDefinitionMapper;
        private readonly IAuthorizationService _authorizationService;
        private readonly INotifier _notifier;

        private readonly dynamic New;
        private readonly IStringLocalizer S;
        private readonly IHtmlLocalizer H;

        public WorkflowDefinitionController
        (
            IWorkflowDefinitionStore workflowDefinitionStore,
            IDistributedLockProvider distributedLockProvider, 
            IWorkflowDefinitionPublisher workflowDefinitionPublisher,
            WorkflowDefinitionMapper workflowDefinitionMapper,
            IOptions<PagerOptions> pagerOptions,
            IAuthorizationService authorizationService,
            IShapeFactory shapeFactory,
            INotifier notifier,
            IStringLocalizer<WorkflowDefinitionController> s,
            IHtmlLocalizer<WorkflowDefinitionController> h)
        {
            _pagerOptions = pagerOptions.Value;
            _workflowDefinitionStore = workflowDefinitionStore;
            _authorizationService = authorizationService;
            _notifier = notifier;

            New = shapeFactory;
            S = s;
            H = h;
            _workflowDefinitionMapper = workflowDefinitionMapper;
            _distributedLockProvider = distributedLockProvider;
            _workflowDefinitionPublisher = workflowDefinitionPublisher;
        }


        [HttpGet("workflow-definitions")]
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

            var latestDefinitionsFilter = new WorkflowDefinitionFilter
            {
                VersionOptions = VersionOptions.Latest
            };

            var pageArgs = new PageArgs(pager.Page - 1, pager.PageSize); // Elsa uses zero-based pages.
            var latestWorkflowDefinitions = await _workflowDefinitionStore.FindSummariesAsync(latestDefinitionsFilter, pageArgs);
            var unpublishedWorkflowDefinitionIds = latestWorkflowDefinitions.Items.Where(x => !x.IsPublished).Select(x => x.DefinitionId).ToList();
            var publishedDefinitionsFilter = new WorkflowDefinitionFilter
            {
                DefinitionIds = unpublishedWorkflowDefinitionIds,
            };
            var publishedWorkflowDefinitions = await _workflowDefinitionStore.FindSummariesAsync(publishedDefinitionsFilter, pageArgs);
            var count = latestWorkflowDefinitions.TotalCount;

            var workflowDefinitionEntries = latestWorkflowDefinitions.Items
                .Select(definition =>
                {
                    var latestVersionNumber = definition.Version;
                    var isPublished = definition.IsPublished;
                    var publishedVersion = isPublished
                        ? definition
                        : publishedWorkflowDefinitions.Items.FirstOrDefault(x => x.DefinitionId == definition.DefinitionId);
                    var publishedVersionNumber = publishedVersion?.Version?.ToString() ?? "-";

                    return new WorkflowDefinitionEntry
                    {
                        WorkflowDefinitionSummary = definition,
                        Id = definition.DefinitionId,
                        WorkflowInstanceCount = 0,
                        Name = definition.Name
                    };
                })
                .ToList();

            // Maintain previous route data when generating page links.
            var routeData = new RouteData();
            routeData.Values.Add("Options.Filter", options.Filter);
            routeData.Values.Add("Options.Search", options.Search);
            routeData.Values.Add("Options.Order", options.Order);

            var pagerShape = (await New.Pager(pager)).TotalItemCount(count).RouteData(routeData);

            var model = new WorkflowDefinitionIndexViewModel
            {
                WorkflowDefinitions = workflowDefinitionEntries,
                Options = options,
                Pager = pagerShape
            };

            model.Options.WorkflowDefinitionsBulkAction = new List<SelectListItem>()
            {
                new() { Text = S["Delete"].Value, Value = nameof(WorkflowDefinitionBulkAction.Delete) }
            };

            return View(model);
        }

        [HttpPost("workflow-definitions"), ActionName("Index")]
        [FormValueRequired("submit.Filter")]
        public ActionResult IndexFilterPOST(WorkflowDefinitionIndexViewModel model)
        {
            return RedirectToAction(nameof(Index), new RouteValueDictionary
            {
                { "Options.Search", model.Options.Search }
            });
        }

        [HttpPost("workflow-definitions/bulk-action")]
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

        [HttpPost("workflow-definitions/delete/{definitionId}")]
        public async Task<IActionResult> Delete(string definitionId, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
                return Forbid();

            var filter = new WorkflowDefinitionFilter
            {
                DefinitionId = definitionId
            };
            
            var workflowDefinition = await _workflowDefinitionStore.FindAsync(filter, cancellationToken);
            
            if (workflowDefinition == null)
                return NotFound();

            await _workflowDefinitionStore.DeleteAsync(filter, cancellationToken);
            await _notifier.SuccessAsync(H["Workflow {0} deleted", workflowDefinition.Name!]);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("workflow-definitions/edit/properties/{definitionId?}")]
        public async Task<IActionResult> EditProperties(string? definitionId, string? returnUrl = null)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
                return Forbid();

            if (definitionId == null)
            {
                return View(new WorkflowDefinitionPropertiesViewModel
                {
                    ReturnUrl = returnUrl
                });
            }

            var workflowDefinition = await _workflowDefinitionStore.FindAsync(new WorkflowDefinitionFilter { DefinitionId = definitionId });

            if (workflowDefinition == null)
                return NotFound();

            return View(new WorkflowDefinitionPropertiesViewModel
            {
                DefinitionId = workflowDefinition.DefinitionId,
                Name = workflowDefinition.Name ?? string.Empty,
                ReturnUrl = returnUrl
            });
        }

        [HttpPost("workflow-definitions/edit/properties/{definitionId?}")]
        public async Task<IActionResult> EditProperties(WorkflowDefinitionPropertiesViewModel viewModel, string? definitionId, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
                return Forbid();

            if (!ModelState.IsValid)
                return View(viewModel);
            
            var resourceName = $"{GetType().FullName}:{(!string.IsNullOrWhiteSpace(definitionId) ? definitionId : Guid.NewGuid().ToString())}";

            await using var handle = await _distributedLockProvider.AcquireLockAsync(resourceName, TimeSpan.FromMinutes(1), cancellationToken);

            var draft = !string.IsNullOrWhiteSpace(definitionId)
                ? await _workflowDefinitionPublisher.GetDraftAsync(definitionId, VersionOptions.Latest, cancellationToken)
                : default;

            var isNew = draft == null;

            // Create a new workflow in case no existing definition was found.
            if (isNew)
            {
                draft = _workflowDefinitionPublisher.New();

                if (!string.IsNullOrWhiteSpace(definitionId))
                    draft.DefinitionId = definitionId;
            }

            draft!.Name = viewModel.Name.Trim();
            await _workflowDefinitionPublisher.SaveDraftAsync(draft, cancellationToken);

            return isNew
                ? RedirectToAction(nameof(Edit), new { draft.DefinitionId })
                : Url.IsLocalUrl(viewModel.ReturnUrl)
                    ? (IActionResult)this.Redirect(viewModel.ReturnUrl, true)
                    : RedirectToAction(nameof(Index));
        }

        [HttpGet("workflow-definitions/edit/{definitionId?}")]
        public ActionResult Edit(string definitionId)
        {
            var serverUrl = new Uri("https://localhost:8092/elsa/api"); // TODO: Get from configuration.
            var viewModel = new WorkflowDefinitionEditViewModel(serverUrl, definitionId);

            return View(viewModel);
        }
    }
}