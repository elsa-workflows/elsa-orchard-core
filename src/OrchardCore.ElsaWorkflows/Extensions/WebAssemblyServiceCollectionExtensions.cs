using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;

namespace OrchardCore.ElsaWorkflows.Extensions;

public static class WebAssemblyServiceCollectionExtensions
{
    /// <summary>
    /// Configures static file options to correctly serve WebAssembly-specific file types,
    /// such as .pdb, .wasm, and .dat, with appropriate MIME types.
    /// </summary>
    /// <param name="services">The service collection used to configure services for the application.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> to allow chaining additional configurations.</returns>
    public static IServiceCollection ConfigureWebAssemblyStaticFiles(this IServiceCollection services)
    {
        services.Configure<StaticFileOptions>(options =>
        {
            var provider = options.ContentTypeProvider as FileExtensionContentTypeProvider
                           ?? new FileExtensionContentTypeProvider();

            provider.Mappings[".pdb"] = "application/octet-stream";
            provider.Mappings[".wasm"] = "application/wasm";
            provider.Mappings[".dat"] = "application/octet-stream"; // you already had this

            options.ContentTypeProvider = provider;
        });
        return services;
    }
}