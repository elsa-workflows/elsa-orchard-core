using Elsa.Common;
using Elsa.Workflows;
using Elsa.Workflows.Activities.Flowchart.Activities;
using Elsa.Workflows.Management;
using Elsa.Workflows.Management.Entities;
using Elsa.Workflows.Management.Materializers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.Admin;

namespace OrchardCore.Elsa.Controllers.WorkflowDefinitions.Create;

public class WorkflowDefinitionsController(
    IAuthorizationService authorizationService, 
    IWorkflowDefinitionStore workflowDefinitionStore,
    ISystemClock systemClock,
    IApiSerializer apiSerializer) : Controller
{
    [Admin("ElsaWorkflows/WorkflowDefinitions/Create/{id}")]
    public async Task<IActionResult> Create(string id)
    {
        if (!await authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
            return Forbid();

        return View(new CreateViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(string id, CreateViewModel viewModel)
    {
        if (!await authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
            return Forbid();

        if (!ModelState.IsValid)
            return View(viewModel);

        var workflowDefinition = new WorkflowDefinition
        {
            CreatedAt = systemClock.UtcNow,
            Name = viewModel.Name.Trim(),
            StringData = apiSerializer.Serialize(new Flowchart()),
            Version = 1,
            IsLatest = true,
            MaterializerName = JsonWorkflowMaterializer.MaterializerName,
            ToolVersion = new(3, 3, 0)
        };
        
        await workflowDefinitionStore.SaveAsync(workflowDefinition);
        
        return RedirectToAction("Edit", new
        {
            Id = workflowDefinition.DefinitionId
        });
    }
}