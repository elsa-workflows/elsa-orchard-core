using Elsa.Email.Contracts;
using Elsa.OrchardCore.Features.Emails.Services;
using Elsa.OrchardCore.Features.Emails.StartupTasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrchardCore.Modules;

namespace Elsa.OrchardCore.Features.Emails;

[Feature("Elsa.OrchardCore.Email")]
public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.Replace(ServiceDescriptor.Scoped<ISmtpService, WrappingSmtpService>());
        services.AddScoped<IModularTenantEvents, RegisterActivitiesStartupTask>();
    }
}