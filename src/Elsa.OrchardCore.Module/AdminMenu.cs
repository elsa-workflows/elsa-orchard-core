using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;

namespace Elsa.OrchardCore;

public class AdminMenu : INavigationProvider
{
    public AdminMenu(IStringLocalizer<AdminMenu> localizer)
    {
        T = localizer;
    }

    private IStringLocalizer T { get; }

    public Task BuildNavigationAsync(string name, NavigationBuilder builder)
    {
        if (!string.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
            return Task.CompletedTask;

        builder
            .Add(T["Elsa Workflows"], NavigationConstants.AdminMenuWorkflowsPosition, workflow =>
            {
                workflow
                    .AddClass("elsa")
                    .Id("elsa")
                    .Add(T["Workflow definitions"], workflowDefinitions => workflowDefinitions
                        .Action("Index", "WorkflowDefinition", new { area = "Elsa.OrchardCore.Module" })
                        .Permission(Permissions.ManageWorkflows)
                        .LocalNav())
                    .Add(T["Workflow instances"], workflowInstances => workflowInstances
                        .Action("Index", "WorkflowInstance", new { area = "Elsa.OrchardCore.Module" })
                        .Permission(Permissions.ManageWorkflows)
                        .LocalNav());
            });

        // builder
        //     .Add(T["Configuration"], configuration => configuration
        //         .AddClass("menu-configuration").Id("configuration")
        //         .Add(T["Elsa Workflows"], workflows => workflows
        //             .Position("50")
        //             .Add(T["Servers"], workflowServers => workflowServers
        //                 .Action("Index", "WorkflowServer", new { area = "Elsa.OrchardCore.Module" })
        //                 .Permission(Permissions.ManageWorkflowServers)
        //                 .LocalNav())));

        return Task.CompletedTask;
    }
}