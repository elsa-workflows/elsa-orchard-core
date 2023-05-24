using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elsa.Extensions;
using Elsa.Workflows.Core;
using Elsa.Workflows.Core.Attributes;
using Elsa.Workflows.Core.Models;
using OrchardCore.ContentManagement;

namespace Elsa.OrchardCore.Features.Contents.Activities.ContentItemCreated;

[Activity("OrchardCore", "Content", "Triggered when a content item is created.", Kind = ActivityKind.Trigger)]
public class ContentItemCreated : Trigger<ContentItem>
{
    [Input(
        DisplayName = "Content type filter",
        Description = "If specified, the workflow will only be triggered if the content item's content type matches one of the specified content types.",
        UIHint = InputUIHints.CheckList,
        OptionsProvider = typeof(ContentTypeOptionsProvider)
    )]
    public Input<ICollection<string>> ContentTypeFilter { get; set; } = new(new List<string>());
    
    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        // Did the workflow start because of this trigger?
        // If yes, then complete the activity and continue with the workflow.
        if (context.IsTriggerOfWorkflow())
        {
            await ExecuteInternalAsync(context);
            return;
        }
        
        // If no, then create a bookmark for each content type filter and wait for the trigger to be invoked.
        var contentTypes = ContentTypeFilter.Get(context);
        foreach (var contentType in contentTypes)
        {
            var payload = new ContentItemCreatedBookmark(contentType);
            context.CreateBookmark(payload, OnResumeAsync);
        }
    }

    protected override IEnumerable<object> GetTriggerPayloads(TriggerIndexingContext context)
    {
        var contentTypes = ContentTypeFilter.Get(context.ExpressionExecutionContext);
        return contentTypes.Select(contentType => new ContentItemCreatedBookmark(contentType)).Cast<object>().ToList();
    }

    private async ValueTask OnResumeAsync(ActivityExecutionContext context)
    {
        await ExecuteInternalAsync(context);
    }
    
    private async ValueTask ExecuteInternalAsync(ActivityExecutionContext context)
    {
        // Get the content item that triggered the workflow.
        var contentItem = context.GetInput<ContentItem>();
            
        // Return the content item as the result of the activity.
        context.SetResult(contentItem);
            
        // Complete the activity.
        await context.CompleteActivityAsync();
    }
}