using Elsa.Workflows.Attributes;

namespace OrchardCore.Elsa.Contents.Activities;

[Activity("OrchardCore.Content", "Content", "Triggered when a content item is created.")]
public class ContentCreated : ContentEventTriggerBase;