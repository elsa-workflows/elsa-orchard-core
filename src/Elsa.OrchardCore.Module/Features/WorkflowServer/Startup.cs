using Elsa.OrchardCore.Features.WorkflowServer.Services;
using Elsa.OrchardCore.Services;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;

namespace Elsa.OrchardCore.Features.WorkflowServer
{
    [Feature("Elsa.OrchardCore.Server")]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IWorkflowServerProvider, LocalWorkflowServerProvider>();
        }
    }
}