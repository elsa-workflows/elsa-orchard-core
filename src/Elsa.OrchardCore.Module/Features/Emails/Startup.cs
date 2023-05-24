using Elsa.Extensions;
using Elsa.OrchardCore.Extensions;
using Elsa.OrchardCore.Features.Emails.Services;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;

namespace Elsa.OrchardCore.Features.Emails;

[Feature("Elsa.OrchardCore.Module.Email")]
public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureElsa(elsa => elsa
            .UseEmail(email => email.SmtpService = sp => sp.GetRequiredService<WrappingSmtpService>())
            .AddActivitiesFrom<Startup>());

        services.AddSingleton<WrappingSmtpService>();
    }
}