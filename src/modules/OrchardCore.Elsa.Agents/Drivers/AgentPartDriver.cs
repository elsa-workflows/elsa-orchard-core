using System.Text.Json;
using Elsa.Agents;
using Elsa.Agents.Persistence.Contracts;
using Elsa.Agents.Persistence.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Elsa.Agents.Parts;
using OrchardCore.Elsa.Agents.ViewModels;
using OrchardCore.Mvc.ModelBinding;

namespace OrchardCore.Elsa.Agents.Drivers;

public sealed class AgentPartDriver : ContentPartDisplayDriver<AgentPart>
{
    private readonly IAgentStore _agentStore;
    private readonly IStringLocalizer<AgentPartDriver> _localizer;

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true
    };

    public AgentPartDriver(
        IAgentStore agentStore,
        IStringLocalizer<AgentPartDriver> localizer)
    {
        _agentStore = agentStore;
        _localizer = localizer;
    }

    public override IDisplayResult Edit(AgentPart part, BuildPartEditorContext context)
    {
        return Initialize<AgentPartViewModel>(GetEditorShapeType(context), model =>
        {
            model.AgentPart = part;
            model.ContentItem = part.ContentItem;
            model.Name = part.Name;
            model.Description = part.Description;

            var config = part.AgentConfig;
            model.FunctionName = config.FunctionName;
            model.PromptTemplate = config.PromptTemplate;
            model.Services = string.Join(", ", config.Services);
            model.Plugins = string.Join(", ", config.Plugins);
            model.Agents = string.Join(", ", config.Agents);
            model.InputVariablesJson = JsonSerializer.Serialize(config.InputVariables ?? new List<InputVariableConfig>(), JsonSerializerOptions);
            model.OutputVariableJson = JsonSerializer.Serialize(config.OutputVariable ?? new OutputVariableConfig(), JsonSerializerOptions);
            model.ExecutionSettingsJson = JsonSerializer.Serialize(config.ExecutionSettings ?? new ExecutionSettingsConfig(), JsonSerializerOptions);
        });
    }

    public override async Task<IDisplayResult> UpdateAsync(AgentPart part, UpdatePartEditorContext context)
    {
        var viewModel = new AgentPartViewModel();

        await context.Updater.TryUpdateModelAsync(viewModel, Prefix, 
            x => x.Name, 
            x => x.Description, 
            x => x.FunctionName,
            x => x.PromptTemplate, 
            x => x.Services, 
            x => x.Plugins, 
            x => x.Agents,
            x => x.InputVariablesJson, 
            x => x.OutputVariableJson, 
            x => x.ExecutionSettingsJson);

        // Validate required fields
        if (string.IsNullOrWhiteSpace(viewModel.Name))
        {
            context.Updater.ModelState.AddModelError(Prefix, nameof(viewModel.Name), _localizer["Name is required."]);
        }

        if (string.IsNullOrWhiteSpace(viewModel.FunctionName))
        {
            context.Updater.ModelState.AddModelError(Prefix, nameof(viewModel.FunctionName), _localizer["Function name is required."]);
        }

        if (string.IsNullOrWhiteSpace(viewModel.PromptTemplate))
        {
            context.Updater.ModelState.AddModelError(Prefix, nameof(viewModel.PromptTemplate), _localizer["Prompt template is required."]);
        }

        // Parse JSON fields
        var inputVariables = Deserialize<List<InputVariableConfig>>(viewModel.InputVariablesJson, nameof(viewModel.InputVariablesJson), context.Updater.ModelState) ?? new List<InputVariableConfig>();
        var outputVariable = Deserialize<OutputVariableConfig>(viewModel.OutputVariableJson, nameof(viewModel.OutputVariableJson), context.Updater.ModelState) ?? new OutputVariableConfig();
        var executionSettings = Deserialize<ExecutionSettingsConfig>(viewModel.ExecutionSettingsJson, nameof(viewModel.ExecutionSettingsJson), context.Updater.ModelState) ?? new ExecutionSettingsConfig();

        if (!context.Updater.ModelState.IsValid)
        {
            return await EditAsync(part, context);
        }

        // Update part
        part.Name = viewModel.Name.Trim();
        part.Description = viewModel.Description?.Trim() ?? string.Empty;
        part.AgentConfig = new AgentConfig
        {
            Name = viewModel.Name.Trim(),
            Description = viewModel.Description?.Trim() ?? string.Empty,
            FunctionName = viewModel.FunctionName.Trim(),
            PromptTemplate = viewModel.PromptTemplate,
            Services = Split(viewModel.Services),
            Plugins = Split(viewModel.Plugins),
            Agents = Split(viewModel.Agents),
            InputVariables = inputVariables,
            OutputVariable = outputVariable,
            ExecutionSettings = executionSettings
        };

        // Create or update agent definition
        var definition = new AgentDefinition
        {
            Id = part.AgentId,
            Name = part.Name,
            Description = part.Description,
            AgentConfig = part.AgentConfig
        };

        if (string.IsNullOrEmpty(part.AgentId))
        {
            await _agentStore.AddAsync(definition);
            part.AgentId = definition.Id!;
        }
        else
        {
            await _agentStore.UpdateAsync(definition);
        }

        return await EditAsync(part, context);
    }

    private T? Deserialize<T>(string json, string fieldName, ModelStateDictionary modelState)
    {
        if (string.IsNullOrWhiteSpace(json))
            return default;

        try
        {
            return JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
        }
        catch (JsonException ex)
        {
            modelState.AddModelError(Prefix, fieldName, _localizer["Invalid JSON: {0}", ex.Message]);
            return default;
        }
    }

    private static List<string> Split(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return [];

        return value
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
