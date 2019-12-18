using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elsa.OrchardCore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IAuthorizationService _authorizationService;

        public WorkflowDefinitionsController(
            ISiteService siteService, 
            IShapeFactory shapeFactory, 
            IAuthorizationService authorizationService,
            IStringLocalizer<WorkflowDefinitionsController> s,
            IHtmlLocalizer<WorkflowDefinitionsController> h)
        {
            _siteService = siteService;
            _authorizationService = authorizationService;
            New = shapeFactory;
            S = s;
            H = h;
        }

        public dynamic New { get; set; }
        public IStringLocalizer<WorkflowDefinitionsController> S { get; }
        public IHtmlLocalizer<WorkflowDefinitionsController> H { get; }

        public async Task<ActionResult> Index(WorkflowDefinitionListOptions options, PagerParameters pagerParameters)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
                return Unauthorized();

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

            var workflowDefinitionEntries = new List<WorkflowDefinitionListEntry>();

            // Maintain previous route data when generating page links.
            var routeData = new RouteData();
            routeData.Values.Add("Options.Filter", options.Filter);
            routeData.Values.Add("Options.Search", options.Search);
            routeData.Values.Add("Options.Order", options.Order);

            var pagerShape = (await New.Pager(pager)).TotalItemCount(count).RouteData(routeData);
            var model = new WorkflowDefinitionListViewModel
            {
                WorkflowDefinitions = workflowDefinitionEntries
                    .Select(x => new WorkflowDefinitionListEntry()
                    {
                        WorkflowDefinition = x.WorkflowDefinition,
                        WorkflowInstanceCount = 0
                    })
                    .ToList(),
                Options = options,
                Pager = pagerShape
            };

            model.Options.BulkActions = new List<SelectListItem>()
            {
                new SelectListItem() { Text = S["Delete"].Value, Value = nameof(WorkflowDefinitionListBulkAction.Delete) }
            };

            return View(model);
        }
    }
}