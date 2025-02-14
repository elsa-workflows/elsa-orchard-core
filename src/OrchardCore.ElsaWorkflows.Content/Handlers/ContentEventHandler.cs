using System.Collections.Generic;
using System.Threading.Tasks;
using Elsa.Workflows.Runtime;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.ElsaWorkflows.Content.Activities;
using OrchardCore.ElsaWorkflows.Content.Stimuli;

namespace OrchardCore.ElsaWorkflows.Content.Handlers;

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