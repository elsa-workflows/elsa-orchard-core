using Elsa.Workflows.Attributes;

namespace OrchardCore.ElsaWorkflows.Contents.Activities;

[Activity("OrchardCore.Content", "Content", "Triggered when a content item draft has been unpublished.")]
public class ContentUnpublished : ContentEventTriggerBase;