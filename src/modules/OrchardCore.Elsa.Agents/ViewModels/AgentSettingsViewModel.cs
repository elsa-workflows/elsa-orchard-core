using Elsa.Agents.Persistence.Entities;

namespace OrchardCore.Elsa.Agents.ViewModels;

public class AgentSettingsViewModel
{
    public IList<ApiKeyDefinition> ApiKeys { get; set; } = [];
    public IList<ServiceDefinition> Services { get; set; } = [];
    public ApiKeyInputModel NewApiKey { get; set; } = new();
    public ServiceInputModel NewService { get; set; } = new();
}

public class ApiKeyInputModel
{
    public string? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class ServiceInputModel
{
    public string? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string SettingsJson { get; set; } = "{}";
}
