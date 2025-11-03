using Elsa.Studio.Core.BlazorServer.Extensions;
using Elsa.Studio.Login.BlazorServer.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OrchardCore.Elsa.Designer.Extensions;

/// <summary>
/// Provides convenience methods for configuring the Elsa Designer for Blazor Server hosts.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Elsa Designer services configured for Blazor Server hosting.
    /// </summary>
    public static IServiceCollection AddElsaDesigner(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddElsaDesignerCore(configuration, platformServices =>
        {
            platformServices.AddCore();
            platformServices.AddLoginModule();
        });
    }
}
