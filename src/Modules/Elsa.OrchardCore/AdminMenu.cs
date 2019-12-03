using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;

namespace Elsa.OrchardCore
{
    public class AdminMenu : INavigationProvider
    {
        public IStringLocalizer T { get; }

        public AdminMenu(IStringLocalizer<AdminMenu> localizer)
        {
            T = localizer;
        }

        public Task BuildNavigationAsync(string name, NavigationBuilder builder)
        {
            if (!string.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
                return Task.CompletedTask;

            builder.Add(T["Elsa"], NavigationConstants.AdminMenuWorkflowsPosition, workflow => workflow
                .AddClass("elsa").Id("elsa").Action("Index", "WorkflowDefinitions", new { area = "Elsa.OrchardCore" })
                    .Permission(Permissions.ManageWorkflows)
                    .LocalNav());

            return Task.CompletedTask;
        }
    }
}
