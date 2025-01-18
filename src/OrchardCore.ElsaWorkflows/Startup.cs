using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.Data;
using OrchardCore.Data.Migration;
using OrchardCore.ElsaWorkflows.Handlers;
using OrchardCore.ElsaWorkflows.Indexes;
using OrchardCore.Modules;
using OrchardCore.Navigation;

namespace OrchardCore.ElsaWorkflows;

public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services
            .AddDataMigration<Migrations>()
            .AddScoped<INavigationProvider, AdminMenu>()
            .AddScoped<IContentHandler, WorkflowDefinitionContentHandler>()
            .AddIndexProvider<WorkflowDefinitionIndexProvider>();
            ;
            
            services.Configure<StaticFileOptions>(options =>
            {
                var provider = new FileExtensionContentTypeProvider
                {
                    Mappings =
                    {
                        // Add custom MIME type mappings
                        [".dat"] = "application/octet-stream" // Adjust the MIME type as needed
                    }
                };
                options.ContentTypeProvider = provider;
            });
    }
}