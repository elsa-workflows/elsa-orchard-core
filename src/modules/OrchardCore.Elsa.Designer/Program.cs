using Elsa.Studio.Shell.Extensions;
using Elsa.Studio.Workflows.Extensions;
using Elsa.Studio.Contracts;
using Elsa.Studio.Core.BlazorWasm.Extensions;
using Elsa.Studio.Extensions;
using Elsa.Studio.Localization.Time;
using Elsa.Studio.Localization.Time.Providers;
using Elsa.Studio.Models;
using Elsa.Studio.Workflows.Designer.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrchardCore.Elsa.Designer;
using OrchardCore.Elsa.Designer.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var configuration = builder.Configuration;

builder.RootComponents.RegisterCustomElsaStudioElements();

builder.Services.AddSingleton<BackendService>();

var backendApiConfig = new BackendApiConfig
{
    ConfigureBackendOptions = options => configuration.GetSection("Backend").Bind(options)
};

builder.Services.AddCore();
builder.Services.AddShell();
builder.Services.AddRemoteBackend(backendApiConfig);
builder.Services.AddScoped<IAuthenticationProviderManager, NoopAuthenticationProviderManager>();
builder.Services.Replace(ServiceDescriptor.Scoped<IRemoteBackendAccessor, ComponentRemoteBackendAccessor>());
builder.Services.AddWorkflowsModule();
builder.Services.AddScoped<ITimeZoneProvider, LocalTimeZoneProvider>();
builder.Services.AddActivityDisplaySettingsProvider<ActivityIconProvider>();

var app = builder.Build();
var startupTask = app.Services.GetServices<IStartupTask>();
foreach (var task in startupTask) await task.ExecuteAsync();
await app.RunAsync();

public class NoopAuthenticationProviderManager : IAuthenticationProviderManager
{
    public Task<string?> GetAuthenticationTokenAsync(string? tokenName, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<string?>(null);
    }
}