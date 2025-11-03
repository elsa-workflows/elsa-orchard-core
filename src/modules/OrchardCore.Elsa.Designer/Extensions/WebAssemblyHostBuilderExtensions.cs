using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace OrchardCore.Elsa.Designer.Extensions;

/// <summary>
/// Extension methods for <see cref="WebAssemblyHostBuilder"/>.
/// </summary>
public static class WebAssemblyHostBuilderExtensions
{
    /// <summary>
    /// Configures the Elsa Designer by registering root components and adding all required services.
    /// </summary>
    /// <param name="builder">The WebAssembly host builder.</param>
    /// <returns>The WebAssembly host builder for chaining.</returns>
    public static WebAssemblyHostBuilder AddElsaDesigner(this WebAssemblyHostBuilder builder)
    {
        builder.RootComponents.RegisterElsaDesignerComponents();
        builder.Services.AddElsaDesigner(builder.Configuration);

        return builder;
    }
}
