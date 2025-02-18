using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elsa.Expressions.Models;
using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using Elsa.Workflows.UIHints;
using OrchardCore.ContentManagement;
using OrchardCore.ElsaWorkflows.Contents.Stimuli;
using OrchardCore.ElsaWorkflows.Contents.UIHints;

namespace OrchardCore.ElsaWorkflows.Contents.Activities;

public abstract class ContentEventTriggerBase : Trigger<ContentItem>
{
    [Input(
        DisplayName = "Content Types",
        Description = "The content types to trigger on.",
        UIHint = InputUIHints.CheckList,
        UIHandler = typeof(ContentTypeCheckListOptionsProvider)
    )]
    public Input<ICollection<string>> ContentTypes { get; set; } = null!;

    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        if (context.IsTriggerOfWorkflow())
            await ExecuteInternalAsync(context);
        else
            context.CreateBookmarks(GetExpectedStimuli(context.ExpressionExecutionContext), ExecuteInternalAsync, false);
    }
    
    protected override ValueTask<IEnumerable<object>> GetTriggerPayloadsAsync(TriggerIndexingContext context)
    {
        return new(GetExpectedStimuli(context.ExpressionExecutionContext));
    }
    
    private IEnumerable<object> GetExpectedStimuli(ExpressionExecutionContext context)
    {
        var contentTypes = ContentTypes.GetOrDefault(context) ?? [];
        return contentTypes.Select(x => new ContentEventStimulus(x)).ToList();
    }

    private async ValueTask ExecuteInternalAsync(ActivityExecutionContext context)
    {
        var contentItem = context.GetWorkflowInput<ContentItem>();
        context.SetResult(contentItem);
        await context.CompleteActivityAsync();
    }
}