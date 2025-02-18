using System.Collections.Generic;
using System.Threading.Tasks;
using Elsa.Workflows.Runtime;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.ElsaWorkflows.Contents.Activities;
using OrchardCore.ElsaWorkflows.Contents.Stimuli;

namespace OrchardCore.ElsaWorkflows.Contents.Handlers;

public class ContentEventHandler(IStimulusSender stimulusSender) : ContentHandlerBase
{
    public override async Task CreatedAsync(CreateContentContext context)
    {
        var contentItem = context.ContentItem;
        var contentType = contentItem.ContentType;
        var stimulus = new ContentCreatedStimulus(contentType);

        await stimulusSender.SendAsync<ContentCreated>(stimulus, new()
        {
            Input = new Dictionary<string, object>
            {
                ["ContentItem"] = contentItem
            }
        });
    }
}