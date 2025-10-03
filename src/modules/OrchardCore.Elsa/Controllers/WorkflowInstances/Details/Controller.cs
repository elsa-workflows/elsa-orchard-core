using Elsa.Extensions;
using Elsa.Workflows.Management;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using OrchardCore.Admin;

namespace OrchardCore.Elsa.Controllers.WorkflowInstances.Details;

[Admin]
public sealed class WorkflowInstancesController(
    IWorkflowInstanceStore workflowInstanceStore,
    IAuthorizationService authorizationService,
    IHtmlLocalizer<WorkflowInstancesController> htmlLocalizer,
    IStringLocalizer<WorkflowInstancesController> stringLocalizer)
    : Controller
{
    internal readonly IHtmlLocalizer H = htmlLocalizer;
    internal readonly IStringLocalizer S = stringLocalizer;

    [Admin("ElsaWorkflows/WorkflowInstances/Details/{id}")]
    public async Task<IActionResult> Details(string id, CancellationToken cancellationToken = default)
    {
        if (!await authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
            return Forbid();
    
        var workflowInstance = await workflowInstanceStore.FindAsync(id, cancellationToken: cancellationToken);
    
        if (workflowInstance == null)
            return NotFound();
    
        var viewModel = new WorkflowInstanceViewModel
        {
            WorkflowInstance = workflowInstance
        };
    
        return View(viewModel);
    }
}
