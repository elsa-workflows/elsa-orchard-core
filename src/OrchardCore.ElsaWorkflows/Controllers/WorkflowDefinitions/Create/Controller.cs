using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.Admin;
using OrchardCore.ContentManagement;
using OrchardCore.ElsaWorkflows.Controllers.WorkflowDefinitions.Create;
using OrchardCore.Title.Models;

namespace OrchardCore.ElsaWorkflows.Endpoints.WorkflowDefinitions.Create;

public class WorkflowDefinitionsController(IAuthorizationService authorizationService, IContentManager contentManager) : Controller
{
    [Admin("ElsaWorkflows/WorkflowDefinitions/Create")]
    public async Task<IActionResult> Create()

    {
        if (!await authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
            return Forbid();

        return View(new CreateViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateViewModel viewModel)
    {
        if (!await authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
            return Forbid();

        if (!ModelState.IsValid)
            return View(viewModel);

        var contentItem = await contentManager.NewAsync("WorkflowDefinition");
        contentItem.Alter<TitlePart>(part => part.Title = viewModel.Name.Trim());
        await contentManager.CreateAsync(contentItem, VersionOptions.Draft);

        return RedirectToAction("Edit", new
        {
            contentItem.ContentItemId
        });
    }
}