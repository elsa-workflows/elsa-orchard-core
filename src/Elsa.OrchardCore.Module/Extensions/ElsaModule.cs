using System;
using Elsa.Extensions;
using Elsa.Features;
using Elsa.Features.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Elsa.OrchardCore.Extensions;

public static class ServiceCollectionExtensions
{
    private static IModule? _elsa;
    
    public static IServiceCollection ConfigureElsa(this IServiceCollection services, Action<IModule>? configure = default)
    {
        var module = GetOrCreateElsaModule(services);
        configure?.Invoke(module);
        module.Apply();
        return services;
    }

    private static IModule GetOrCreateElsaModule(IServiceCollection services)
    {
        if (_elsa != null) 
            return _elsa;
        
        _elsa = services.CreateModule();
        _elsa.Configure<ElsaFeature>();

        return _elsa;
    }
}