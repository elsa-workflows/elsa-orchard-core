using System.Threading.Tasks;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.ElsaWorkflows.Parts;

namespace OrchardCore.ElsaWorkflows.Handlers.Content;

public class WorkflowDefinitionContentHandler : ContentHandlerBase
{
    
    
    public override Task GetContentItemAspectAsync(ContentItemAspectContext context)
    {
        if(!context.ContentItem.Has<WorkflowDefinitionPart>())
            return Task.CompletedTask;
        
        return context.ForAsync<ContentItemMetadata>(metadata =>
        {
            metadata.CreateRouteValues = new()
            {
                { "Area", Constants.Area },
                { "Controller", "WorkflowDefinitions" },
                { "Action", "Create" },
                { "ContentType", context.ContentItem.ContentType },
            };

            metadata.EditorRouteValues = new()
            {
                { "Area", Constants.Area },
                { "Controller", "WorkflowDefinitions" },
                { "Action", "Edit" },
                { "ContentItemId", context.ContentItem.ContentItemId },
            };

            metadata.AdminRouteValues = new()
            {
                { "Area", Constants.Area },
                { "Controller", "WorkflowDefinitions" },
                { "Action", "Edit" },
                { "ContentItemId", context.ContentItem.ContentItemId },
            };

            return Task.CompletedTask;
        });
    }
}
