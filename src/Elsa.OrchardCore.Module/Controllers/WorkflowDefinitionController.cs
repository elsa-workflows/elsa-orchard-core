using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Client.Models;
using Elsa.OrchardCore.Services;
using Elsa.OrchardCore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using OrchardCore.Admin;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Navigation;
using OrchardCore.Routing;
using OrchardCore.Settings;

namespace Elsa.OrchardCore.Controllers
{
    [Admin]
    [Route("admin/elsa/server/{serverId}/workflow-definition")]
    public class WorkflowDefinitionController : Controller
    {
        private readonly IWorkflowServerManager _workflowServerManager;
        private readonly IWorkflowServerClientFactory _workflowServerClientFactory;
        private readonly ISiteService _siteService;
        private readonly IAuthorizationService _authorizationService;
        private readonly INotifier _notifier;

        public WorkflowDefinitionController
        (
            IWorkflowServerManager workflowServerManager,
            IWorkflowServerClientFactory workflowServerClientFactory,
            ISiteService siteService,
            IAuthorizationService authorizationService,
            IShapeFactory shapeFactory,
            INotifier notifier,
            IStringLocalizer<WorkflowDefinitionController> s,
            IHtmlLocalizer<WorkflowDefinitionController> h)
        {
            _workflowServerManager = workflowServerManager;
            _workflowServerClientFactory = workflowServerClientFactory;
            _siteService = siteService;
            _authorizationService = authorizationService;
            _notifier = notifier;

            New = shapeFactory;
            S = s;
            H = h;
        }

        private dynamic New { get; }
        private IStringLocalizer S { get; }
        private IHtmlLocalizer H { get; }

        [HttpGet("index")]
        public async Task<IActionResult> Index(string serverId, WorkflowDefinitionIndexOptions? options, PagerParameters pagerParameters, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
            {
                return Forbid();
            }

            var siteSettings = await _siteService.GetSiteSettingsAsync();
            var pager = new Pager(pagerParameters, siteSettings.PageSize);

            if (options == null)
                options = new WorkflowDefinitionIndexOptions();

            var workflowServerClient = await _workflowServerClientFactory.CreateClientAsync(serverId, cancellationToken);
            var workflowDefinitionPagedList = await workflowServerClient.ListWorkflowDefinitionsAsync(pagerParameters.Page, pagerParameters.PageSize, VersionOptions.All, cancellationToken);
            
            var workflowGroups = workflowDefinitionPagedList.Items
                .GroupBy(x => x.DefinitionId)
                .ToDictionary(x => x.Key);
            
            var count = workflowGroups.Count;

            // Maintain previous route data when generating page links.
            var routeData = new RouteData();
            routeData.Values.Add("Options.Filter", options.Filter);
            routeData.Values.Add("Options.Search", options.Search);
            routeData.Values.Add("Options.Order", options.Order);

            var pagerShape = (await New.Pager(pager)).TotalItemCount(count).RouteData(routeData);

            var entries = workflowGroups
                .Select(grouping =>
                {
                    var workflowDefinitionVersions = grouping;
                    var latestVersion = workflowDefinitionVersions.Value.Where(x => x.IsLatest).OrderByDescending(x => x.Version).First();
                    var publishedVersion = workflowDefinitionVersions.Value.Where(x => x.IsPublished).OrderByDescending(x => x.Version).FirstOrDefault();

                    return new WorkflowDefinitionListEntry
                    {
                        Description = latestVersion.Description,
                        Id = latestVersion.Id,
                        Name = latestVersion.Name ?? "Untitled",
                        DefinitionId = latestVersion.DefinitionId,
                        LatestVersion = latestVersion.Version,
                        PublishedVersion = publishedVersion?.Version
                    };
                })
                .ToList();

            var model = new WorkflowDefinitionIndexViewModel
            {
                ServerId = serverId,
                WorkflowDefinitions = entries,
                Options = options,
                Pager = pagerShape
            };

            options.WorkflowTypesBulkAction = new List<SelectListItem>()
            {
                new() {Text = S["Delete"].Value, Value = nameof(WorkflowDefinitionBulkAction.Delete)}
            };

            return View(model);
        }

        [HttpPost, ActionName("Index")]
        [FormValueRequired("submit.Filter")]
        public ActionResult IndexFilterPOST(WorkflowDefinitionIndexViewModel model)
        {
            return RedirectToAction(nameof(Index), new RouteValueDictionary
            {
                {"serverId", model.ServerId},
                {"Options.Search", model.Options.Search}
            });
        }

        [HttpPost]
        [FormValueRequired("submit.BulkAction")]
        public async Task<IActionResult> BulkEdit(string serverId, WorkflowDefinitionIndexOptions options, IEnumerable<int>? itemIds)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
            {
                return Forbid();
            }

            var itemIdList = itemIds?.ToList();

            if (itemIdList?.Count > 0)
            {
                switch (options.BulkAction)
                {
                    case WorkflowDefinitionBulkAction.None:
                        break;
                    case WorkflowDefinitionBulkAction.Delete:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return RedirectToAction(nameof(Index), new {serverId});
        }

        [HttpGet("{id}/properties", Name = "EditProperties")]
        public async Task<IActionResult> EditProperties(string serverId, string? id, string? returnUrl = default, CancellationToken cancellationToken = default)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
                return Forbid();

            if (id == null)
                return View(new WorkflowDefinitionPropertiesViewModel
                {
                    ReturnUrl = returnUrl
                });

            var workflowServerClient = await _workflowServerClientFactory.CreateClientAsync(serverId, cancellationToken);
            var workflowDefinition = await workflowServerClient.GetWorkflowDefinitionAsync(id, VersionOptions.Latest, cancellationToken);

            return View(new WorkflowDefinitionPropertiesViewModel
            {
                Id = workflowDefinition!.Id,
                Name = workflowDefinition.Name ?? "Untitled",
                IsSingleton = workflowDefinition.IsSingleton,
                DeleteCompletedInstances = workflowDefinition.DeleteCompletedInstances,
                ReturnUrl = returnUrl
            });
        }

        [HttpPost("{id}/properties")]
        public async Task<IActionResult> EditProperties(string serverId, WorkflowDefinitionPropertiesViewModel viewModel, string? id, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
                return Forbid();

            if (!ModelState.IsValid)
                return View(viewModel);

            var isNew = id == null;
            WorkflowDefinition? workflowDefinition;
            var workflowServerClient = await _workflowServerClientFactory.CreateClientAsync(serverId, cancellationToken);

            if (isNew)
                workflowDefinition = new WorkflowDefinition();
            else
            {
                workflowDefinition = await workflowServerClient.GetWorkflowDefinitionAsync(id!, cancellationToken);

                if (workflowDefinition == null)
                    return NotFound();
            }

            workflowDefinition.Name = viewModel.Name?.Trim();
            workflowDefinition.IsSingleton = viewModel.IsSingleton;
            workflowDefinition.DeleteCompletedInstances = viewModel.DeleteCompletedInstances;

            var saveRequest = new SaveWorkflowDefinitionRequest
            {
                Activities = workflowDefinition.Activities,
                Connections = workflowDefinition.Connections,
                Description = workflowDefinition.Description,
                Name = workflowDefinition.Name,
                Publish = false,
                Variables = workflowDefinition.Variables,
                ContextOptions = workflowDefinition.ContextOptions,
                DisplayName = workflowDefinition.DisplayName,
                IsSingleton = workflowDefinition.IsSingleton,
                PersistenceBehavior = workflowDefinition.PersistenceBehavior,
                DeleteCompletedInstances = workflowDefinition.DeleteCompletedInstances,
                WorkflowDefinitionId = workflowDefinition.DefinitionId
            };

            await workflowServerClient.SaveWorkflowDefinitionAsync(saveRequest, cancellationToken);

            return isNew
                ? RedirectToAction(nameof(Edit), new {serverId, workflowDefinition.Id})
                : Url.IsLocalUrl(viewModel.ReturnUrl)
                    ? Redirect(viewModel.ReturnUrl)
                    : RedirectToAction(nameof(Index));
        }

        [HttpGet("{id}/duplicate")]
        public async Task<IActionResult> Duplicate(string serverId, string id, string? returnUrl = null, CancellationToken cancellationToken = default)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
                return Forbid();

            var workflowServerClient = await _workflowServerClientFactory.CreateClientAsync(serverId, cancellationToken);
            var workflowDefinition = await workflowServerClient.GetWorkflowDefinitionAsync(id, cancellationToken);

            if (workflowDefinition == null)
                return NotFound();

            return View(new WorkflowDefinitionPropertiesViewModel
            {
                Id = id,
                IsSingleton = workflowDefinition.IsSingleton,
                Name = "Copy-" + workflowDefinition.Name,
                ReturnUrl = returnUrl
            });
        }

        [HttpPost("{id}/duplicate")]
        public async Task<IActionResult> Duplicate(string serverId, WorkflowDefinitionPropertiesViewModel viewModel, string id, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
                return Forbid();

            if (!ModelState.IsValid)
                return View(viewModel);

            var workflowServerClient = await _workflowServerClientFactory.CreateClientAsync(serverId, cancellationToken);
            var existingWorkflowDefinition = await workflowServerClient.GetWorkflowDefinitionAsync(id, cancellationToken);

            if (existingWorkflowDefinition == null)
                return NotFound();

            var workflowDefinition = new SaveWorkflowDefinitionRequest
            {
                Name = viewModel.Name.Trim(),
                DisplayName = existingWorkflowDefinition.DisplayName,
                Activities = existingWorkflowDefinition.Activities,
                Connections = existingWorkflowDefinition.Connections,
                Description = existingWorkflowDefinition.Description,
                Variables = existingWorkflowDefinition.Variables,
                ContextOptions = existingWorkflowDefinition.ContextOptions,
                IsSingleton = viewModel.IsSingleton,
                DeleteCompletedInstances = viewModel.DeleteCompletedInstances,
                PersistenceBehavior = existingWorkflowDefinition.PersistenceBehavior
            };

            var savedWorkflowDefinition = await workflowServerClient.SaveWorkflowDefinitionAsync(workflowDefinition, cancellationToken);

            return RedirectToAction(nameof(Edit), new {serverId, savedWorkflowDefinition.Id});
        }

        [HttpGet("{id}/edit")]
        public async Task<IActionResult> Edit(string serverId, string id, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
                return Forbid();

            var workflowServer = await _workflowServerManager.GetWorkflowServerAsync(serverId, cancellationToken);
            var workflowServerClient = await _workflowServerClientFactory.CreateClientAsync(workflowServer!, cancellationToken);
            var workflowDefinition = await workflowServerClient.GetWorkflowDefinitionAsync(id, cancellationToken);

            if (workflowDefinition == null)
                return NotFound();

            var viewModel = new WorkflowDefinitionViewModel
            {
                WorkflowDefinition = workflowDefinition,
                ServerId = serverId,
                ServerUrl = await workflowServer!.GetServerUrlAsync(cancellationToken)
            };

            return View(viewModel);
        }

        [HttpPost("{id}/edit")]
        public async Task<IActionResult> Delete(string serverId, string id, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
                return Forbid();

            var workflowServerClient = await _workflowServerClientFactory.CreateClientAsync(serverId, cancellationToken);
            var workflowDefinition = await workflowServerClient.GetWorkflowDefinitionAsync(id, cancellationToken);

            if (workflowDefinition == null)
                return NotFound();

            await workflowServerClient.DeleteWorkflowDefinitionAsync(workflowDefinition.DefinitionId, cancellationToken);
            _notifier.Success(H["Workflow {0} deleted", workflowDefinition.Name]);

            return RedirectToAction(nameof(Index), new { serverId });
        }
    }
}