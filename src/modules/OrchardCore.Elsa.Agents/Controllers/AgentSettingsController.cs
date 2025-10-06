using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Elsa.Agents.Persistence.Contracts;
using Elsa.Agents.Persistence.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using OrchardCore.Admin;
using OrchardCore.Elsa.Agents.ViewModels;

namespace OrchardCore.Elsa.Agents.Controllers;

[Admin("ElsaWorkflows/Agents/Settings")]
[Area(Constants.Area)]
public class AgentSettingsController(
    IAuthorizationService authorizationService,
    IApiKeyStore apiKeyStore,
    IServiceStore serviceStore,
    IStringLocalizer<AgentSettingsController> localizer) : Controller
{
    private IStringLocalizer T { get; } = localizer;

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        if (!await AuthorizeAsync())
            return Forbid();

        var model = await BuildViewModelAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddApiKey(ApiKeyInputModel model)
    {
        if (!await AuthorizeAsync())
            return Forbid();

        if (string.IsNullOrWhiteSpace(model.Name))
            ModelState.AddModelError(nameof(model.Name), T["Name is required."]);

        if (string.IsNullOrWhiteSpace(model.Value))
            ModelState.AddModelError(nameof(model.Value), T["Value is required."]);

        if (!ModelState.IsValid)
        {
            var viewModel = await BuildViewModelAsync();
            viewModel.NewApiKey = model;
            return View("Edit", viewModel);
        }

        var entity = new ApiKeyDefinition
        {
            Name = model.Name.Trim(),
            Value = model.Value.Trim()
        };

        await apiKeyStore.AddAsync(entity);
        return RedirectToAction(nameof(Edit));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateApiKey(ApiKeyInputModel model)
    {
        if (!await AuthorizeAsync())
            return Forbid();

        if (string.IsNullOrWhiteSpace(model.Id))
            ModelState.AddModelError(string.Empty, T["Identifier is required."]);

        if (string.IsNullOrWhiteSpace(model.Name))
            ModelState.AddModelError(nameof(model.Name), T["Name is required."]);

        if (string.IsNullOrWhiteSpace(model.Value))
            ModelState.AddModelError(nameof(model.Value), T["Value is required."]);

        if (!ModelState.IsValid)
        {
            var viewModel = await BuildViewModelAsync();
            viewModel.NewApiKey = new ApiKeyInputModel();
            return View("Edit", viewModel);
        }

        var entity = new ApiKeyDefinition
        {
            Id = model.Id,
            Name = model.Name.Trim(),
            Value = model.Value.Trim()
        };

        await apiKeyStore.UpdateAsync(entity);
        return RedirectToAction(nameof(Edit));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteApiKey(string id)
    {
        if (!await AuthorizeAsync())
            return Forbid();

        if (!string.IsNullOrWhiteSpace(id))
        {
            var entity = await apiKeyStore.GetAsync(id);
            if (entity != null)
                await apiKeyStore.DeleteAsync(entity);
        }

        return RedirectToAction(nameof(Edit));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddService(ServiceInputModel model)
    {
        if (!await AuthorizeAsync())
            return Forbid();

        ValidateService(model);
        var settings = ParseSettings(model.SettingsJson, nameof(model.SettingsJson));

        if (!ModelState.IsValid)
        {
            var viewModel = await BuildViewModelAsync();
            viewModel.NewService = model;
            return View("Edit", viewModel);
        }

        var entity = new ServiceDefinition
        {
            Name = model.Name.Trim(),
            Type = model.Type.Trim(),
            Settings = settings ?? new Dictionary<string, object>()
        };

        await serviceStore.AddAsync(entity);
        return RedirectToAction(nameof(Edit));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateService(ServiceInputModel model)
    {
        if (!await AuthorizeAsync())
            return Forbid();

        if (string.IsNullOrWhiteSpace(model.Id))
            ModelState.AddModelError(string.Empty, T["Identifier is required."]);

        ValidateService(model);
        var settings = ParseSettings(model.SettingsJson, nameof(model.SettingsJson));

        if (!ModelState.IsValid)
        {
            var viewModel = await BuildViewModelAsync();
            return View("Edit", viewModel);
        }

        var entity = new ServiceDefinition
        {
            Id = model.Id,
            Name = model.Name.Trim(),
            Type = model.Type.Trim(),
            Settings = settings ?? new Dictionary<string, object>()
        };

        await serviceStore.UpdateAsync(entity);
        return RedirectToAction(nameof(Edit));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteService(string id)
    {
        if (!await AuthorizeAsync())
            return Forbid();

        if (!string.IsNullOrWhiteSpace(id))
        {
            var entity = await serviceStore.GetAsync(id);
            if (entity != null)
                await serviceStore.DeleteAsync(entity);
        }

        return RedirectToAction(nameof(Edit));
    }

    private async Task<AgentSettingsViewModel> BuildViewModelAsync()
    {
        var apiKeys = (await apiKeyStore.ListAsync()).OrderBy(x => x.Name).ToList();
        var services = (await serviceStore.ListAsync()).OrderBy(x => x.Name).ToList();

        return new AgentSettingsViewModel
        {
            ApiKeys = apiKeys,
            Services = services,
            NewApiKey = new ApiKeyInputModel(),
            NewService = new ServiceInputModel()
        };
    }

    private void ValidateService(ServiceInputModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Name))
            ModelState.AddModelError(nameof(model.Name), T["Name is required."]);

        if (string.IsNullOrWhiteSpace(model.Type))
            ModelState.AddModelError(nameof(model.Type), T["Type is required."]);
    }

    private IDictionary<string, object>? ParseSettings(string json, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new Dictionary<string, object>();

        try
        {
            var dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return dictionary ?? new Dictionary<string, object>();
        }
        catch (JsonException ex)
        {
            ModelState.AddModelError(fieldName, T["Invalid JSON: {0}", ex.Message]);
            return null;
        }
    }

    private async Task<bool> AuthorizeAsync() => await authorizationService.AuthorizeAsync(User, Permissions.ManageAgents);
}
