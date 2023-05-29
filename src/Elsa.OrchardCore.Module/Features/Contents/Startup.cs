using Elsa.Extensions;
using Elsa.JavaScript.Extensions;
using Elsa.Liquid.Notifications;
using Elsa.OrchardCore.Extensions;
using Elsa.OrchardCore.Features.Contents.Activities.ContentItemCreated;
using Elsa.OrchardCore.Features.Contents.Scripting;
using Elsa.OrchardCore.Services;
using Elsa.Workflows.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.Modules;

namespace Elsa.OrchardCore.Features.Contents;

[Feature("Elsa.OrchardCore.Module.Contents")]
public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureElsa(elsa =>
        {
            elsa.AddActivitiesFrom<Startup>();
            elsa.UseWorkflowManagement(management =>
            {
                management.AddVariableType<ContentItem>("Content");
            });
            elsa.UseLiquid();
        });
        
        services.AddScoped<IActivityPropertyOptionsProvider, ContentTypeOptionsProvider>();
        services.AddScoped<IContentHandler, WorkflowContentItemHandler>();
        services.AddTypeDefinitionProvider<ContentDefinitionProvider>();
    }
}