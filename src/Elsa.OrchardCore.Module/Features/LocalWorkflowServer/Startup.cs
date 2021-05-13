using Elsa.OrchardCore.Features.LocalWorkflowServer.Services;
using Elsa.OrchardCore.Services;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;

namespace Elsa.OrchardCore.Features.LocalWorkflowServer
{
    [Feature("Elsa.OrchardCore.LocalWorkflowServer")]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IWorkflowServerProvider, LocalWorkflowServerProvider>();
        }
    }
}