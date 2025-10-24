using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;

namespace OrchardCore.Elsa.Agents;

public class AdminMenu(IStringLocalizer<AdminMenu> localizer) : INavigationProvider
{
    private IStringLocalizer T { get; } = localizer;

    public ValueTask BuildNavigationAsync(string name, NavigationBuilder builder)
    {
        if (!string.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
            return ValueTask.CompletedTask;

        if (NavigationHelper.UseLegacyFormat())
        {
            builder
                .Add(T["Elsa"], NavigationConstants.AdminMenuWorkflowsPosition, menu =>
                {
                    menu
                        .AddClass("elsa-agents")
                        .Id(AgentConstants.AgentAdminMenuId)
                        .Add(T["Agents"], item => item
                            .Action("List", "Admin", new { area = "OrchardCore.Contents", contentTypeId = AgentConstants.AgentContentType })
                            .Permission(Permissions.ManageAgents)
                            .LocalNav())
                        .Add(T["Settings"], item => item
                            .Action("Edit", "AgentSettings", new { area = Constants.Area })
                            .Permission(Permissions.ManageAgents)
                            .LocalNav());
                });

            return ValueTask.CompletedTask;
        }

        builder
            .Add(T["Tools"], tools => tools
                .Add(T["Elsa"], T["WorkflowsB"].PrefixPosition(), workflow =>
                {
                    workflow
                        .AddClass("elsa-agents")
                        .Id(AgentConstants.AgentAdminMenuId)
                        .Add(T["Agents"], item => item
                            .Action("List", "Admin", new { area = "OrchardCore.Contents", contentTypeId = AgentConstants.AgentContentType })
                            .Permission(Permissions.ManageAgents)
                            .LocalNav())
                        .Add(T["Settings"], item => item
                            .Action("Edit", "AgentSettings", new { area = Constants.Area })
                            .Permission(Permissions.ManageAgents)
                            .LocalNav());
                }));

        return ValueTask.CompletedTask;
    }
}
