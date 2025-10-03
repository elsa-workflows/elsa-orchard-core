using System.Text.Json.Nodes;
using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Activities.Flowchart.Attributes;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using Elsa.Workflows.UIHints;
using JetBrains.Annotations;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.Elsa.Contents.UIHints;

namespace OrchardCore.Elsa.Contents.Activities;

[Activity("OrchardCore.Content", "Content", "Create a new content item.")]
[FlowNode("Created", "Validation Error")]
[UsedImplicitly]
public class CreateContent : CodeActivity<ContentItem>
{
    [Input(
        Description = "The content type to create.",
        UIHint = InputUIHints.DropDown,
        UIHandler = typeof(ContentTypeOptionsProvider)
    )]
    public Input<string> ContentType { get; set; } = null!;

    [Input(
        Description = "Whether to publish the content item after creation.",
        UIHint = InputUIHints.Checkbox
    )]
    public Input<bool> Publish { get; set; } = null!;

    [Input(
        Description = "The content properties to set.",
        UIHint = InputUIHints.JsonEditor
    )]
    public Input<JsonObject> ContentProperties { get; set; } = null!;

    [Output(Description = "The validation result of the content item.")] public Output<ContentValidateResult> ValidationResult { get; set; } = null!;

    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        var contentManager = context.GetRequiredService<IContentManager>();
        var contentType = ContentType.Get(context);
        var contentProperties = ContentProperties.GetOrDefault(context);
        var contentItem = await contentManager.NewAsync(contentType);

        if (contentProperties != null) 
            contentItem.Merge(contentProperties);

        var result = await contentManager.ValidateAsync(contentItem);
        ValidationResult.Set(context, result);

        if (result.Succeeded)
        {
            await contentManager.CreateAsync(contentItem, VersionOptions.Draft);
            var publish = Publish.Get(context);

            if (publish)
                await contentManager.PublishAsync(contentItem);
            else
                await contentManager.SaveDraftAsync(contentItem);

            context.SetResult(contentItem);
            await context.CompleteActivityWithOutcomesAsync("Created");
            return;
        }

        await context.CompleteActivityWithOutcomesAsync("Validation Error");
    }
}