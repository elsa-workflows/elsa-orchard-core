using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.Admin;
using OrchardCore.ContentManagement;

namespace OrchardCore.ElsaWorkflows.Controllers.WorkflowDefinitions.Edit;

public class WorkflowDefinitionsController(IAuthorizationService authorizationService, IContentManager contentManager) : Controller
{
    [Admin("ElsaWorkflows/WorkflowDefinitions/Edit/{contentItemId}")]
    public async Task<IActionResult> Edit(string contentItemId)

    {
        if (!await authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
            return Forbid();

        return View(new EditViewModel
        {
            DefinitionId = contentItemId
        });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string id, EditViewModel viewModel)
    {
        if (!await authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
            return Forbid();

        if (!ModelState.IsValid)
            return View(viewModel);
        
        return RedirectToAction("List");
    }
}