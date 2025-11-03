using Elsa.Studio.Contracts;
using Elsa.Studio.Core.BlazorWasm.Extensions;
using Elsa.Studio.Extensions;
using Elsa.Studio.Localization.Time;
using Elsa.Studio.Localization.Time.Providers;
using Elsa.Studio.Models;
using Elsa.Studio.Shell.Extensions;
using Elsa.Studio.Workflows.Designer.Extensions;
using Elsa.Studio.Workflows.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrchardCore.Elsa.Designer.Services;

namespace OrchardCore.Elsa.Designer.Extensions;

/// <summary>
/// Extension methods for configuring the Elsa Designer in a Blazor WebAssembly application.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Elsa Designer services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration to bind backend options from.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddElsaDesigner(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<BackendService>();

        var backendApiConfig = new BackendApiConfig
        {
            ConfigureBackendOptions = options => configuration.GetSection("Backend").Bind(options)
        };

        services.AddCore();
        services.AddShell();
        services.AddRemoteBackend(backendApiConfig);
        services.AddScoped<IAuthenticationProviderManager, NoopAuthenticationProviderManager>();
        services.Replace(ServiceDescriptor.Scoped<IRemoteBackendAccessor, ComponentRemoteBackendAccessor>());
        services.AddWorkflowsModule();
        services.AddScoped<ITimeZoneProvider, LocalTimeZoneProvider>();
        services.AddActivityDisplaySettingsProvider<ActivityIconProvider>();

        return services;
    }

    /// <summary>
    /// Registers custom Elsa Studio elements as root components.
    /// </summary>
    /// <param name="rootComponents">The root component mappings.</param>
    /// <returns>The root component mappings.</returns>
    public static RootComponentMappingCollection RegisterElsaDesignerComponents(this RootComponentMappingCollection rootComponents)
    {
        rootComponents.RegisterCustomElsaStudioElements();
        return rootComponents;
    }
}

/// <summary>
/// A no-op authentication provider manager for use in embedded scenarios where authentication is handled by the host.
/// </summary>
public class NoopAuthenticationProviderManager : IAuthenticationProviderManager
{
    public Task<string?> GetAuthenticationTokenAsync(string? tokenName, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<string?>(null);
    }
}
