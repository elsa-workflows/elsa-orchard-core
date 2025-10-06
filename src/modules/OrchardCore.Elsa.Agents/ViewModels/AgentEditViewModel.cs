namespace OrchardCore.Elsa.Agents.ViewModels;

public class AgentEditViewModel
{
    public string? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string FunctionName { get; set; } = string.Empty;
    public string PromptTemplate { get; set; } = string.Empty;
    public string Services { get; set; } = string.Empty;
    public string Plugins { get; set; } = string.Empty;
    public string Agents { get; set; } = string.Empty;
    public string InputVariablesJson { get; set; } = string.Empty;
    public string OutputVariableJson { get; set; } = string.Empty;
    public string ExecutionSettingsJson { get; set; } = string.Empty;
}
