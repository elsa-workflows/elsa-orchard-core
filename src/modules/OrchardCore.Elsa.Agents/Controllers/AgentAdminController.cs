using System.Text.Json;
using Elsa.Agents;
using Elsa.Agents.Persistence.Contracts;
using Elsa.Agents.Persistence.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using OrchardCore.Admin;
using OrchardCore.ContentManagement;
using OrchardCore.DisplayManagement;
using OrchardCore.Elsa.Agents.Indexes;
using OrchardCore.Elsa.Agents.Parts;
using OrchardCore.Elsa.Agents.ViewModels;
using OrchardCore.Navigation;
using YesSql;
using VersionOptions = OrchardCore.ContentManagement.VersionOptions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OrchardCore.Elsa.Agents.Controllers;

[Admin("ElsaWorkflows/Agents")]
[Area(Constants.Area)]
public class AgentAdminController(
    IAuthorizationService authorizationService,
    ISession session,
    IContentManager contentManager,
    IAgentStore agentStore,
    IShapeFactory shapeFactory,
    IStringLocalizer<AgentAdminController> localizer,
    IOptions<PagerOptions> pagerOptions) : Controller
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true
    };

    private IStringLocalizer T { get; } = localizer;

    public async Task<IActionResult> List(AgentListOptions? options, PagerParameters pagerParameters)
    {
        if (!await AuthorizeAsync())
            return Forbid();

        options ??= new();
        var pager = new Pager(pagerParameters, pagerOptions.Value.GetPageSize());
        var query = session.Query<ContentItem, AgentIndex>(index => index.Latest);

        if (!string.IsNullOrWhiteSpace(options.Search))
            query = query.Where(index => index.Name!.Contains(options.Search));

        query = options.Filter switch
        {
            AgentFilter.Published => query.Where(index => index.Published),
            AgentFilter.Draft => query.Where(index => !index.Published),
            _ => query
        };

        query = query.OrderBy(index => index.Name);

        var count = await query.CountAsync();
        var contentItems = await query
            .Skip(pager.GetStartIndex())
            .Take(pager.PageSize)
            .ListAsync();

        var items = contentItems
            .Select(contentItem =>
            {
                var part = contentItem.As<AgentPart>() ?? new AgentPart();
                return new AgentSummaryViewModel
                {
                    AgentId = part.AgentId,
                    Name = part.Name,
                    Description = part.Description,
                    IsPublished = contentItem.Published,
                    IsLatest = contentItem.Latest
                };
            })
            .ToList();

        var routeData = new RouteData();
        if (!string.IsNullOrEmpty(options.Search))
            routeData.Values.TryAdd("Options.Search", options.Search);
        routeData.Values.Add("Options.Filter", options.Filter);

        var pagerShape = await shapeFactory.PagerAsync(pager, count, routeData);
        options.BulkActions =
        [
            new(T["Publish"], nameof(AgentBulkAction.Publish)),
            new(T["Unpublish"], nameof(AgentBulkAction.Unpublish)),
            new(T["Delete"], nameof(AgentBulkAction.Delete))
        ];

        var model = new AgentListViewModel
        {
            Items = items,
            Options = options,
            Pager = pagerShape
        };

        return View(model);
    }

    [HttpPost]
    [ActionName(nameof(List))]
    public async Task<IActionResult> BulkAction(AgentListOptions options, PagerParameters pagerParameters, string[] itemIds)
    {
        if (!await AuthorizeAsync())
            return Forbid();

        if (options.BulkAction == AgentBulkAction.None || itemIds.Length == 0)
            return RedirectToAction(nameof(List), BuildRouteValues(options));

        foreach (var agentId in itemIds)
        {
            var contentItem = await GetContentItemByAgentIdAsync(agentId, VersionOptions.Latest);
            if (contentItem == null)
                continue;

            switch (options.BulkAction)
            {
                case AgentBulkAction.Publish:
                    await contentManager.PublishAsync(contentItem);
                    break;
                case AgentBulkAction.Unpublish:
                    await contentManager.UnpublishAsync(contentItem);
                    break;
                case AgentBulkAction.Delete:
                    await contentManager.RemoveAsync(contentItem);
                    break;
            }
        }

        return RedirectToAction(nameof(List), BuildRouteValues(options));
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        if (!await AuthorizeAsync())
            return Forbid();

        var viewModel = new AgentEditViewModel
        {
            InputVariablesJson = JsonSerializer.Serialize(new List<InputVariableConfig>(), JsonSerializerOptions),
            OutputVariableJson = JsonSerializer.Serialize(new OutputVariableConfig(), JsonSerializerOptions),
            ExecutionSettingsJson = JsonSerializer.Serialize(new ExecutionSettingsConfig(), JsonSerializerOptions)
        };

        return View("Edit", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AgentEditViewModel model, string? submit)
    {
        if (!await AuthorizeAsync())
            return Forbid();

        var definition = await BuildDefinitionAsync(model);
        if (!ModelState.IsValid)
            return View("Edit", model);

        await agentStore.AddAsync(definition);
        await HandleSubmissionAsync(definition.Id!, submit);

        return RedirectToAction(nameof(Edit), new { id = definition.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        if (!await AuthorizeAsync())
            return Forbid();

        var definition = await agentStore.GetAsync(id);
        if (definition == null)
            return NotFound();

        var model = Map(definition);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AgentEditViewModel model, string? submit)
    {
        if (!await AuthorizeAsync())
            return Forbid();

        var definition = await BuildDefinitionAsync(model);
        if (!ModelState.IsValid)
            return View(model);

        await agentStore.UpdateAsync(definition);
        await HandleSubmissionAsync(definition.Id!, submit);

        return RedirectToAction(nameof(Edit), new { id = definition.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        if (!await AuthorizeAsync())
            return Forbid();

        var contentItem = await GetContentItemByAgentIdAsync(id, VersionOptions.Latest);
        if (contentItem != null)
            await contentManager.RemoveAsync(contentItem);

        return RedirectToAction(nameof(List));
    }

    private async Task HandleSubmissionAsync(string agentId, string? submit)
    {
        if (string.IsNullOrEmpty(submit))
            return;

        var contentItem = await GetContentItemByAgentIdAsync(agentId, VersionOptions.Latest);
        if (contentItem == null)
            return;

        if (string.Equals(submit, "publish", StringComparison.OrdinalIgnoreCase))
            await contentManager.PublishAsync(contentItem);
        else if (string.Equals(submit, "unpublish", StringComparison.OrdinalIgnoreCase))
            await contentManager.UnpublishAsync(contentItem);
    }

    private async Task<AgentDefinition> BuildDefinitionAsync(AgentEditViewModel model)
    {
        var services = Split(model.Services);
        var plugins = Split(model.Plugins);
        var agents = Split(model.Agents);

        if (string.IsNullOrWhiteSpace(model.Name))
            ModelState.AddModelError(nameof(model.Name), T["Name is required."]);

        if (string.IsNullOrWhiteSpace(model.FunctionName))
            ModelState.AddModelError(nameof(model.FunctionName), T["Function name is required."]);

        if (string.IsNullOrWhiteSpace(model.PromptTemplate))
            ModelState.AddModelError(nameof(model.PromptTemplate), T["Prompt template is required."]);

        var inputVariables = Deserialize<List<InputVariableConfig>>(model.InputVariablesJson, nameof(model.InputVariablesJson)) ?? new List<InputVariableConfig>();
        var outputVariable = Deserialize<OutputVariableConfig>(model.OutputVariableJson, nameof(model.OutputVariableJson)) ?? new OutputVariableConfig();
        var executionSettings = Deserialize<ExecutionSettingsConfig>(model.ExecutionSettingsJson, nameof(model.ExecutionSettingsJson)) ?? new ExecutionSettingsConfig();

        if (!ModelState.IsValid)
            return new AgentDefinition();

        return new AgentDefinition
        {
            Id = model.Id,
            Name = model.Name.Trim(),
            Description = model.Description.Trim(),
            AgentConfig = new AgentConfig
            {
                Name = model.Name.Trim(),
                Description = model.Description.Trim(),
                FunctionName = model.FunctionName.Trim(),
                PromptTemplate = model.PromptTemplate,
                Services = services,
                Plugins = plugins,
                Agents = agents,
                InputVariables = inputVariables,
                OutputVariable = outputVariable,
                ExecutionSettings = executionSettings
            }
        };
    }

    private AgentEditViewModel Map(AgentDefinition definition)
    {
        var config = definition.AgentConfig ?? new AgentConfig();
        return new AgentEditViewModel
        {
            Id = definition.Id,
            Name = definition.Name,
            Description = definition.Description,
            FunctionName = config.FunctionName ?? string.Empty,
            PromptTemplate = config.PromptTemplate ?? string.Empty,
            Services = string.Join(", ", config.Services ?? Array.Empty<string>()),
            Plugins = string.Join(", ", config.Plugins ?? Array.Empty<string>()),
            Agents = string.Join(", ", config.Agents ?? Array.Empty<string>()),
            InputVariablesJson = JsonSerializer.Serialize(config.InputVariables ?? new List<InputVariableConfig>(), JsonSerializerOptions),
            OutputVariableJson = JsonSerializer.Serialize(config.OutputVariable ?? new OutputVariableConfig(), JsonSerializerOptions),
            ExecutionSettingsJson = JsonSerializer.Serialize(config.ExecutionSettings ?? new ExecutionSettingsConfig(), JsonSerializerOptions)
        };
    }

    private async Task<bool> AuthorizeAsync() => await authorizationService.AuthorizeAsync(User, Permissions.ManageAgents);

    private async Task<ContentItem?> GetContentItemByAgentIdAsync(string agentId, VersionOptions version)
    {
        if (string.IsNullOrWhiteSpace(agentId))
            return null;

        var index = await session.QueryIndex<AgentIndex>()
            .Where(x => x.AgentId == agentId && (version == VersionOptions.Published ? x.Published : x.Latest))
            .FirstOrDefaultAsync();

        if (index == null)
            return null;

        return await contentManager.GetAsync(index.ContentItemId, version);
    }

    private RouteValueDictionary BuildRouteValues(AgentListOptions options)
    {
        var values = new RouteValueDictionary();
        if (!string.IsNullOrWhiteSpace(options.Search))
            values["Options.Search"] = options.Search;
        values["Options.Filter"] = options.Filter;
        return values;
    }

    private static List<string> Split(string value)
    {
        return value
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private T? Deserialize<T>(string json, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(json))
            return default;

        try
        {
            return JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
        }
        catch (JsonException ex)
        {
            ModelState.AddModelError(fieldName, T["Invalid JSON: {0}", ex.Message]);
            return default;
        }
    }
}
