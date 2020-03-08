using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.OrchardCore.Services;
using Elsa.OrchardCore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using OrchardCore.Admin;
using OrchardCore.DisplayManagement;
using OrchardCore.Navigation;
using OrchardCore.Settings;

namespace Elsa.OrchardCore.Controllers
{
    [Admin]
    [Route("admin/elsa/workflow-definitions")]
    public class WorkflowDefinitionsController : Controller
    {
        private readonly ISiteService _siteService;
        private readonly IElsaClientFactory _elsaClientFactory;
        private readonly IWorkflowServerService _workflowServerService;
        private readonly IAuthorizationService _authorizationService;

        public WorkflowDefinitionsController(
            ISiteService siteService,
            IElsaClientFactory elsaClientFactoryFactory,
            IWorkflowServerService workflowServerService,
            IShapeFactory shapeFactory,
            IAuthorizationService authorizationService,
            IStringLocalizer<WorkflowDefinitionsController> localizer)
        {
            _siteService = siteService;
            _elsaClientFactory = elsaClientFactoryFactory;
            _workflowServerService = workflowServerService;
            _authorizationService = authorizationService;
            New = shapeFactory;
            T = localizer;
        }

        private dynamic New { get; set; }
        private IStringLocalizer<WorkflowDefinitionsController> T { get; }

        [HttpGet("{server}")]
        public async Task<ActionResult> Index(string server, WorkflowDefinitionListOptions options, PagerParameters pagerParameters, CancellationToken cancellationToken)
        {
            var workflowServer = await _workflowServerService.GetWorkflowServerAsync(server, cancellationToken);

            if (workflowServer == null)
                return NotFound();

            var client = _elsaClientFactory.GetOrCreateClient(workflowServer.Url);

            var workflowDefinitions = await client.GetWorkflowDefinitionsAsync(
                new VersionOptionsInput
                {
                    LatestOrPublished = true
                }, cancellationToken);

            var siteSettings = await _siteService.GetSiteSettingsAsync();
            var pager = new Pager(pagerParameters, siteSettings.PageSize);

            if (options == null) options = new WorkflowDefinitionListOptions();

            switch (options.Filter)
            {
                default:
                    break;
            }

            if (!string.IsNullOrWhiteSpace(options.Search))
            {
                //query = query.Where(w => w.Name.Contains(options.Search));
            }

            switch (options.Order)
            {
//                case WorkflowTypeOrder.Name:
//                    query = query.OrderBy(u => u.Name);
//                    break;
            }

            var count = 0;

            var groupings = workflowDefinitions.Data.WorkflowDefinitions.GroupBy(x => x.DefinitionId);
            var entries = groupings.Select(grouping =>
            {
                var latest = grouping.First(x => x.IsLatest);
                var published = grouping.FirstOrDefault(x => x.IsPublished);
                var publishedVersion = published?.Version.ToString();
                var name = !string.IsNullOrWhiteSpace(latest.Name) ? latest.Name : default;
                var description = latest.Description;
                var latestVersion = latest.Version;
                
                return new WorkflowDefinitionListEntry
                {
                    Id = latest.Id,
                    Name = name,
                    Description = description,
                    PublishedVersion = publishedVersion,
                    LatestVersion = latestVersion,
                    DefinitionId = grouping.Key,
                    IsDisabled = latest.IsDisabled,
                    WorkflowInstanceCount = 0
                };
            }).ToList();

            // Maintain previous route data when generating page links.
            var routeData = new RouteData();
            routeData.Values.Add("Options.Filter", options.Filter);
            routeData.Values.Add("Options.Search", options.Search);
            routeData.Values.Add("Options.Order", options.Order);

            var pagerShape = (await New.Pager(pager)).TotalItemCount(count).RouteData(routeData);
            var model = new WorkflowDefinitionListViewModel
            {
                WorkflowDefinitions = entries,
                Options = options,
                Pager = pagerShape
            };

            model.Options.BulkActions = new List<SelectListItem>()
            {
                new SelectListItem() { Text = T["Delete"].Value, Value = nameof(WorkflowDefinitionListBulkAction.Delete) }
            };

            return View(model);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpGet("edit/{id}")]
        public IActionResult Edit(string id)
        {
            return View();
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflowServers))
                context.Result = Forbid();
            else
                await next();
        }
    }
}