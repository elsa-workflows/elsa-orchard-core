using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.OrchardCore.Models;
using Elsa.OrchardCore.Services;
using Elsa.OrchardCore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using OrchardCore.Admin;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Entities;

namespace Elsa.OrchardCore.Controllers
{
    [Admin]
    [Route("admin/elsa/workflow-servers")]
    public class WorkflowServersController : Controller
    {
        private readonly IWorkflowServerStore _store;
        private readonly IAuthorizationService _authorizationService;
        private readonly IIdGenerator _idGenerator;
        private readonly INotifier _notifier;

        public WorkflowServersController(
            IWorkflowServerStore store,
            IAuthorizationService authorizationService,
            IIdGenerator idGenerator,
            INotifier notifier,
            IHtmlLocalizer<WorkflowDefinitionsController> localizer)
        {
            _store = store;
            _authorizationService = authorizationService;
            _idGenerator = idGenerator;
            _notifier = notifier;
            T = localizer;
        }

        private IHtmlLocalizer<WorkflowDefinitionsController> T { get; }

        [HttpGet("")]
        public async Task<ActionResult> Index(CancellationToken cancellationToken)
        {
            var servers = await _store.ListAsync(cancellationToken);
            var viewModel = new WorkflowServersViewModel
            {
                WorkflowServers = servers.ToList()
            };

            return View(viewModel);
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(string id, CancellationToken cancellationToken)
        {
            var workflowServer = await _store.GetByIdAsync(id, cancellationToken);

            if (workflowServer == null)
                return NotFound();

            var model = new WorkflowServerEditModel
            {
                Name = workflowServer.Name,
                Url = workflowServer.Url
            };

            return View(model);
        }

        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Edit(string id, WorkflowServerEditModel model, string returnUrl, CancellationToken cancellationToken)
        {
            var workflowServer = await _store.GetByIdAsync(id, cancellationToken);

            if (workflowServer == null)
                return NotFound();

            workflowServer.Name = model.Name.Trim();
            workflowServer.Url = model.Url;

            await _store.SaveAsync(workflowServer, cancellationToken);

            _notifier.Success(T["Remote workflow server has been updated."]);
            return Url.IsLocalUrl(returnUrl) ? (IActionResult) Redirect(returnUrl) : RedirectToAction("Index");
        }

        [HttpGet("create")]
        public IActionResult Create() => View();

        [HttpPost("create")]
        public async Task<IActionResult> Create(WorkflowServerEditModel model, CancellationToken cancellationToken)
        {
            var workflowServer = new WorkflowServer
            {
                WorkflowServerId = _idGenerator.GenerateUniqueId(),
                Name = model.Name.Trim(),
                Url = model.Url
            };

            await _store.SaveAsync(workflowServer, cancellationToken);

            _notifier.Success(T["Remote workflow server has been created."]);
            return RedirectToAction("Index");
        }

        [HttpPost("delete/{id}")]
        public async Task<IActionResult> Delete(string id, string returnUrl, CancellationToken cancellationToken)
        {
            var workflowServer = await _store.GetByIdAsync(id, cancellationToken);

            if (workflowServer == null)
                return NotFound();

            await _store.DeleteAsync(workflowServer, cancellationToken);

            _notifier.Success(T["Remote workflow server has been deleted."]);
            return Url.IsLocalUrl(returnUrl) ? (IActionResult) Redirect(returnUrl) : RedirectToAction("Index");
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