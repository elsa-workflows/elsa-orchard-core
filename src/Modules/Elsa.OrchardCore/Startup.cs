using System;
using Elsa.OrchardCore.Drivers;
using Elsa.OrchardCore.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.Settings;

namespace Elsa.OrchardCore
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services
                .AddScoped<INavigationProvider, AdminMenu>()
                .AddScoped<IDisplayDriver<ISite>, ElsaWorkflowsSettingsDisplayDriver>();
        }

        public override void Configure(
            IApplicationBuilder builder,
            IEndpointRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}