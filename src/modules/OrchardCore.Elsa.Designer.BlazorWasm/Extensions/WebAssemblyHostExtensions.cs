using Elsa.Studio.Contracts;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace OrchardCore.Elsa.Designer.Extensions;

/// <summary>
/// Extension methods for <see cref="WebAssemblyHost"/>.
/// </summary>
public static class WebAssemblyHostExtensions
{
    /// <summary>
    /// Executes all registered startup tasks.
    /// </summary>
    /// <param name="host">The WebAssembly host.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task RunStartupTasksAsync(this WebAssemblyHost host, CancellationToken cancellationToken = default)
    {
        var startupTasks = host.Services.GetServices<IStartupTask>();

        foreach (var task in startupTasks)
        {
            await task.ExecuteAsync(cancellationToken);
        }
    }
}
