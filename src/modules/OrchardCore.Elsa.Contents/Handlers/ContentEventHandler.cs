using Elsa.Workflows;
using Elsa.Workflows.Runtime;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.Elsa.Contents.Activities;
using OrchardCore.Elsa.Contents.Stimuli;

namespace OrchardCore.Elsa.Contents.Handlers;

public class ContentEventHandler(IStimulusSender stimulusSender) : ContentHandlerBase
{
    public override Task CreatedAsync(CreateContentContext context) => TriggerActivity<ContentCreated>(context);
    public override Task DraftSavedAsync(SaveDraftContentContext context) => TriggerActivity<ContentDraftSaved>(context);
    public override Task RemovedAsync(RemoveContentContext context) => TriggerActivity<ContentDeleted>(context);
    public override Task PublishedAsync(PublishContentContext context) => TriggerActivity<ContentPublished>(context);
    public override Task UnpublishedAsync(PublishContentContext context) => TriggerActivity<ContentUnpublished>(context);
    public override Task UpdatedAsync(UpdateContentContext context) => TriggerActivity<ContentUpdated>(context);
    public override Task VersionedAsync(VersionContentContext context) => TriggerActivity<ContentVersioned>(context);

    private async Task TriggerActivity<TTriggerActivity>(ContentContextBase context) where TTriggerActivity : IActivity
    {
        var contentItem = context.ContentItem;
        var contentType = contentItem.ContentType;
        var stimulus = new ContentEventStimulus(contentType);

        await stimulusSender.SendAsync<TTriggerActivity>(stimulus, new()
        {
            Input = new Dictionary<string, object>
            {
                ["ContentItem"] = contentItem
            }
        });
    }
}