using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa.OrchardCore.Contracts;
using Elsa.OrchardCore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Localization;
using OrchardCore.Admin;

namespace Elsa.OrchardCore.Controllers;

[Admin]
[Route("admin/elsa/workflow-studio")]
public class WorkflowStudioController : Controller
{
    private readonly IWorkflowServerManager _workflowServerManager;
    private readonly IAuthorizationService _authorizationService;

    public WorkflowStudioController(
        IWorkflowServerManager workflowServerManager,
        IAuthorizationService authorizationService,
        IHtmlLocalizer<WorkflowServersController> localizer)
    {
        _workflowServerManager = workflowServerManager;
        _authorizationService = authorizationService;
        T = localizer;
    }

    private IHtmlLocalizer T { get; }

    [HttpGet("{serverId}")]
    public async Task<ActionResult> Index(string serverId, CancellationToken cancellationToken)
    {
        var server = await _workflowServerManager.GetServerAsync(serverId, cancellationToken);
            
        if (server == null)
            return NotFound();
            
        var serverUrl = await server.GetServerUrlAsync(cancellationToken);
        var viewModel = new WorkflowStudioViewModel(serverUrl);

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