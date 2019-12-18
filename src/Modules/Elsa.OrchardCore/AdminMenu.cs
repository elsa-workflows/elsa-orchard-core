using System;
using System.Threading.Tasks;
using Elsa.OrchardCore.Drivers;
using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;

namespace Elsa.OrchardCore
{
    public class AdminMenu : INavigationProvider
    {
        public AdminMenu(IStringLocalizer<AdminMenu> localizer)
        {
            T = localizer;
        }

        public IStringLocalizer T { get; }

        public Task BuildNavigationAsync(string name, NavigationBuilder builder)
        {
            if (!string.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
                return Task.CompletedTask;

            builder
                .Add(T["Elsa"], NavigationConstants.AdminMenuWorkflowsPosition, workflow => workflow
                    .AddClass("elsa").Id("elsa").Action("Index", "WorkflowDefinitions", new { area = "Elsa.OrchardCore" })
                    .Permission(Permissions.ManageWorkflows)
                    .LocalNav())
                .Add(T["Configuration"], configuration => configuration
                    .Add(T["Settings"], settings => settings
                        .Add(T["Elsa Workflows"], elsa => elsa
                            .Action("Index", "Admin", new { area = "OrchardCore.Settings", groupId = ElsaWorkflowsSettingsDisplayDriver.GroupId })
                            .Permission(Permissions.ManageWorkflows)
                            .LocalNav()
                        )));

            return Task.CompletedTask;
        }
    }
}