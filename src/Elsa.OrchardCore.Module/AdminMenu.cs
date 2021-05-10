using System;
using System.Linq;
using System.Threading.Tasks;
using Elsa.OrchardCore.Services;
using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;

namespace Elsa.OrchardCore
{
    public class AdminMenu : INavigationProvider
    {
        private readonly IWorkflowServerService _workflowServerService;

        public AdminMenu(IStringLocalizer<AdminMenu> localizer, IWorkflowServerService workflowServerService)
        {
            _workflowServerService = workflowServerService;
            T = localizer;
        }

        private IStringLocalizer T { get; }

        public async Task BuildNavigationAsync(string name, NavigationBuilder builder)
        {
            if (!string.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
                return;

            var servers = (await _workflowServerService.ListWorkflowServersAsync()).ToList();

            if (servers.Count == 1)
            {
                var server = servers.First();

                builder
                    .Add(T["Workflows"], NavigationConstants.AdminMenuWorkflowsPosition, workflow => workflow
                        .AddClass("elsa")
                        .Id("elsa")
                        .Action("Index", "WorkflowDashboard", new { area = "Elsa.OrchardCore.Module", serverId = server.WorkflowServerId })
                        .Permission(Permissions.ManageWorkflows)
                        .LocalNav());
            }
            else
            {
                foreach (var server in servers)
                {
                    builder
                        .Add(T["Workflows"], NavigationConstants.AdminMenuWorkflowsPosition, workflows => workflows
                            .AddClass("elsa")
                            .Id("elsa")
                            .Add(T[server.Name],  workflow => workflow
                                .Action("Index", "WorkflowDashboard", new { area = "Elsa.OrchardCore.Module", serverId = server.WorkflowServerId })
                                .Permission(Permissions.ManageWorkflows)
                                .LocalNav()));
                }
            }

            builder
                .Add(T["Configuration"], configuration => configuration
                    .AddClass("menu-configuration").Id("configuration")
                    .Add(T["Workflows"], workflows => workflows
                        .Position("50")
                        .Add(T["Servers"], workflowServers => workflowServers
                            .Action("Index", "WorkflowServers", new { area = "Elsa.OrchardCore.Module" })
                            .Permission(Permissions.ManageWorkflowServers)
                            .LocalNav())));
        }
    }
}