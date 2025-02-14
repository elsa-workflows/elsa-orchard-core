using System.Threading.Tasks;
using Elsa.Workflows.Activities.Flowchart.Activities;
using Elsa.Workflows.Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.Admin;
using OrchardCore.ContentManagement;
using OrchardCore.ElsaWorkflows.Parts;
using OrchardCore.ElsaWorkflows.Services;
using OrchardCore.Title.Models;

namespace OrchardCore.ElsaWorkflows.Controllers.WorkflowDefinitions.Create;

public class WorkflowDefinitionsController(IAuthorizationService authorizationService, IContentManager contentManager, WorkflowDefinitionPartMapper definitionPartMapper) : Controller
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

        var contentItem = await contentManager.NewAsync(id);
        await contentManager.CreateAsync(contentItem, VersionOptions.Draft);

        var workflowDefinitionModel = new WorkflowDefinitionModel
        {
            DefinitionId = contentItem.ContentItemId,
            Id = contentItem.ContentItemVersionId,
            CreatedAt = contentItem.CreatedUtc!.Value,
            Name = viewModel.Name.Trim(),
            Root = new Flowchart(),
            Version = 1,
            IsLatest = true,
            ToolVersion = new(3, 3, 0)
        };

        contentItem.Alter<TitlePart>(part => part.Title = viewModel.Name.Trim());
        contentItem.Alter<WorkflowDefinitionPart>(part => { definitionPartMapper.Map(workflowDefinitionModel, part); });
        await contentManager.SaveDraftAsync(contentItem);

        return RedirectToAction("Edit", new
        {
            Id = contentItem.ContentItemId
        });
    }
}