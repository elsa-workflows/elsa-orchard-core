using System.Threading.Tasks;
using Elsa.Extensions;
using Elsa.OrchardCore.Services;
using Elsa.Workflows.Core;
using Elsa.Workflows.Core.Activities.Flowchart.Attributes;
using Elsa.Workflows.Core.Attributes;
using Elsa.Workflows.Core.Models;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using YesSql;

namespace Elsa.OrchardCore.Features.Contents.Activities.CreateContentItem;

[Activity("OrchardCore", "Content", "Create a new content item.", Kind = ActivityKind.Action)]
[FlowNode("Done", "Failed")]
public class CreateContentItem : CodeActivity<ContentItem>
{
    /// <summary>
    /// The content type to use.
    /// </summary>
    [Input(
        Description = "The content type to use.",
        UIHint = InputUIHints.Dropdown,
        OptionsProvider = typeof(ContentTypeOptionsProvider)
    )]
    public Input<string> ContentType { get; set; } = default!;

    /// <summary>
    /// Whether to publish the content item.
    /// </summary>
    [Input(Description = "Whether to publish the content item.")]
    public Input<bool> Publish { get; set; } = default!;

    /// <summary>
    /// A JSON value for the content parts, fields and their properties.
    /// </summary>
    [Input(
        Description = "A JSON value for the content parts, fields and their properties.",
        DefaultValue = "{ \"DisplayText\": \"Enter a title\" }"
    )]
    public Input<string> ContentProperties { get; set; } = default!;

    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        var contentType = ContentType.Get(context);
        var contentProperties = ContentProperties.GetOrDefault(context);
        var contentManager = context.GetRequiredService<IContentManager>();
        var contentItem = await contentManager.NewAsync(contentType);

        if (!string.IsNullOrWhiteSpace(contentProperties))
            contentItem.Merge(JObject.Parse(contentProperties));

        var result = await contentManager.UpdateValidateAndCreateAsync(contentItem, VersionOptions.Draft);

        if (result.Succeeded)
        {
            var publish = Publish.Get(context);

            if (publish)
                await contentManager.PublishAsync(contentItem);
            else
                await contentManager.SaveDraftAsync(contentItem);
            
            context.SetResult(contentItem);
        }

        var outcome = result.Succeeded ? "Done" : "Failed";
        await context.CompleteActivityWithOutcomesAsync(outcome);

        await context.GetRequiredService<ISession>().SaveChangesAsync();
    }
}