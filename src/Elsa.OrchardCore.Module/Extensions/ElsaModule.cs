using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Elsa.Extensions;
using Elsa.Features;
using Elsa.Features.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Elsa.OrchardCore.Extensions;

public static class ServiceCollectionExtensions
{
    private static readonly IDictionary<IServiceCollection, IModule> ElsaContainers = new ConcurrentDictionary<IServiceCollection, IModule>();
    
    public static IModule ConfigureElsa(this IServiceCollection services, Action<IModule>? configure = default)
    {
        var module = GetOrCreateElsaModule(services);
        
        if(configure != null)
            module.Configure<AppFeature>(app => app.Configurator += configure);
        
        return module;
    }

    private static IModule GetOrCreateElsaModule(IServiceCollection services)
    {
        if(ElsaContainers.TryGetValue(services, out var elsa))
            return elsa;
        
        elsa = services.CreateModule();
        
        ElsaContainers[services] = elsa;
        return elsa;
    }
}