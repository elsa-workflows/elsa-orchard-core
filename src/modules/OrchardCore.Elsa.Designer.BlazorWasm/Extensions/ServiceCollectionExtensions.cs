using Elsa.Studio.Core.BlazorWasm.Extensions;
using Elsa.Studio.Login.BlazorWasm.Extensions;
using Elsa.Studio.Workflows.Designer.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OrchardCore.Elsa.Designer.Extensions;

/// <summary>
/// Provides convenience methods for configuring the Elsa Designer for Blazor WebAssembly hosts.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Elsa Designer services configured for Blazor WebAssembly hosting.
    /// </summary>
    public static IServiceCollection AddElsaDesigner(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddElsaDesignerCore(configuration, platformServices =>
        {
            platformServices.AddCore();
            platformServices.AddLoginModule();
        });
    }

    /// <summary>
    /// Registers the Elsa Designer custom elements as root components.
    /// </summary>
    public static RootComponentMappingCollection RegisterElsaDesignerComponents(this RootComponentMappingCollection rootComponents)
    {
        rootComponents.RegisterCustomElsaStudioElements();
        return rootComponents;
    }
}
