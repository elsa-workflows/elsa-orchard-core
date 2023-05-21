using System.Collections.Generic;
using System.Threading.Tasks;
using Elsa.Extensions;
using Elsa.Workflows.Runtime.Contracts;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;

namespace Elsa.OrchardCore.Features.LocalWorkflowServer.Activities.Contents.ContentItemCreated;

public class WorkflowContentItemHandler : ContentHandlerBase
{
    private readonly IWorkflowRuntime _workflowRuntime;

    public WorkflowContentItemHandler(IWorkflowRuntime workflowRuntime)
    {
        _workflowRuntime = workflowRuntime;
    }

    public override async Task CreatedAsync(CreateContentContext context)
    {
        var contentItem = context.ContentItem;
        var contentTypeName = contentItem.ContentType;
        var bookmarkPayload = new ContentItemCreatedBookmark(contentTypeName);
        var input = new Dictionary<string, object> { [nameof(ContentItem)] = contentItem };
        var options = new TriggerWorkflowsRuntimeOptions { Input = input };
        await _workflowRuntime.TriggerWorkflowsAsync<Features.LocalWorkflowServer.Activities.Contents.ContentItemCreated.ContentItemCreated>(bookmarkPayload, options);
    }
}