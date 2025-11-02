using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;

namespace OrchardCore.Elsa;

public class AdminMenu(IStringLocalizer<AdminMenu> localizer) : INavigationProvider
{
    private IStringLocalizer T { get; } = localizer;

    public ValueTask BuildNavigationAsync(string name, NavigationBuilder builder)
    {
        if (!string.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
            return ValueTask.CompletedTask;

        builder
        .Add(T["Elsa Workflows"], NavigationConstants.AdminMenuWorkflowsPosition, workflow =>
        {
            workflow
                .AddClass("elsa")
                .Id("elsa")
                .Add(T["Definitions"], workflowDefinitions => workflowDefinitions
                    .Action("List", "WorkflowDefinitions", new { area = Constants.Area })
                    .Permission(Permissions.ManageWorkflows)
                    .LocalNav())
                .Add(T["Instances"], workflowInstances => workflowInstances
                    .Action("List", "WorkflowInstances", new { area = Constants.Area })
                    .Permission(Permissions.ManageWorkflows)
                    .LocalNav());
        });

        return ValueTask.CompletedTask;
    }
}