using Elsa.Workflows.Attributes;

namespace OrchardCore.Elsa.Contents.Activities;

[Activity("OrchardCore.Content", "Content", "Triggered when a content item has been deleted.")]
public class ContentDeleted : ContentEventTriggerBase;