using System.Threading;
using System.Threading.Tasks;
using Elsa.OrchardCore.Services;
using Elsa.OrchardCore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Localization;
using OrchardCore.Admin;

namespace Elsa.OrchardCore.Controllers
{
    [Admin]
    [Route("admin/elsa/workflow-servers/{serverId}/dashboard")]
    public class WorkflowDashboardController : Controller
    {
        private readonly IWorkflowServerStore _store;
        private readonly IAuthorizationService _authorizationService;

        public WorkflowDashboardController(
            IWorkflowServerStore store,
            IAuthorizationService authorizationService,
            IHtmlLocalizer<WorkflowDashboardController> localizer)
        {
            _store = store;
            _authorizationService = authorizationService;
            T = localizer;
        }

        private IHtmlLocalizer T { get; }

        [HttpGet]
        public async Task<ActionResult> Index(string serverId, CancellationToken cancellationToken)
        {
            var server = await _store.GetByIdAsync(serverId, cancellationToken);

            if (server == null)
                return NotFound();
            
            var viewModel = new DashboardViewModel
            {
                WorkflowServer = server
            };

            return View(viewModel);
        }
        
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
                context.Result = Forbid();
            else
                await next();
        }
    }
}