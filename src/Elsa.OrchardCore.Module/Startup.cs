using System;
using Elsa.OrchardCore.Indexes;
using Elsa.OrchardCore.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Data.Migration;
using OrchardCore.Entities;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using YesSql.Indexes;

namespace Elsa.OrchardCore
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services
                .AddIdGeneration()
                .AddScoped<IDataMigration, Migrations>()
                .AddScoped<INavigationProvider, AdminMenu>()
                .AddScoped<IWorkflowServerManager, WorkflowServerManager>()
                .AddScoped<IWorkflowServerClientFactory, WorkflowServerClientFactory>()
                ;
        }

        public override void Configure(
            IApplicationBuilder builder,
            IEndpointRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}