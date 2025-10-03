using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Activities.Flowchart.Attributes;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using JetBrains.Annotations;
using OrchardCore.ContentManagement;

namespace OrchardCore.Elsa.Contents.Activities;

[Activity("OrchardCore.Content", "Content", "Update an existing content item.")]
[FlowNode("Deleted", "Not Found")]
[UsedImplicitly]
public class DeleteContent : CodeActivity<ContentItem>
{
    [Input(Description = "The content item ID.")] public Input<string> ContentItemId { get; set; } = null!;

    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        var contentManager = context.GetRequiredService<IContentManager>();
        var contentItemId = ContentItemId.Get(context);
        var contentItem = await contentManager.GetAsync(contentItemId, VersionOptions.DraftRequired);

        if (contentItem == null)
        {
            await context.CompleteActivityWithOutcomesAsync("Not Found");
            return;
        }

        await contentManager.RemoveAsync(contentItem);

        context.SetResult(contentItem);
        await context.CompleteActivityWithOutcomesAsync("Deleted");
    }
}