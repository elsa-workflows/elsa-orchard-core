using Elsa.OrchardCore.Features.RemoteWorkflows.Services;
using Elsa.OrchardCore.Indexes;
using Elsa.OrchardCore.Services;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;
using YesSql.Indexes;

namespace Elsa.OrchardCore.Features.RemoteWorkflows
{
    [Feature("Elsa.OrchardCore.RemoteWorkflowServers")]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services
                .AddScoped<IRemoteWorkflowServerStore, RemoteWorkflowServerStore>()
                .AddScoped<IWorkflowServerProvider, RemoteWorkflowServerProvider>()
                .AddSingleton<IIndexProvider, RemoteWorkflowServerIndexProvider>();
        }
    }
}