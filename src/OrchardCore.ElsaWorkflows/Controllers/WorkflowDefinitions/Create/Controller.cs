using System;
using System.Threading.Tasks;
using Elsa.Workflows;
using Elsa.Workflows.Activities.Flowchart.Activities;
using Elsa.Workflows.Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.Admin;
using OrchardCore.ContentManagement;
using OrchardCore.ElsaWorkflows.Parts;
using OrchardCore.Title.Models;

namespace OrchardCore.ElsaWorkflows.Controllers.WorkflowDefinitions.Create;

public class WorkflowDefinitionsController(IAuthorizationService authorizationService, IContentManager contentManager, IApiSerializer apiSerializer) : Controller
{
    [Admin("ElsaWorkflows/WorkflowDefinitions/Create/{contentType}")]
    public async Task<IActionResult> Create(string contentType)
    {
        if (!await authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
            return Forbid();

        return View(new CreateViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(string contentType, CreateViewModel viewModel)
    {
        if (!await authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
            return Forbid();

        if (!ModelState.IsValid)
            return View(viewModel);

        var workflowDefinitionModel = new WorkflowDefinitionModel
        {
            Name = viewModel.Name.Trim(),
            Root = new Flowchart(),
            Version = 1,
            IsLatest = true,
            ToolVersion = new(3, 3, 0)
        };

        var contentItem = await contentManager.NewAsync(contentType);
        contentItem.Alter<TitlePart>(part => part.Title = viewModel.Name.Trim());
        contentItem.Alter<WorkflowDefinitionPart>(part => { part.SerializedData = apiSerializer.Serialize(workflowDefinitionModel); });
        await contentManager.CreateAsync(contentItem, VersionOptions.Draft);

        return RedirectToAction("Edit", new
        {
            contentItem.ContentItemId
        });
    }
}