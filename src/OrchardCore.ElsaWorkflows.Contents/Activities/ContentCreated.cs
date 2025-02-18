using Elsa.Workflows.Attributes;

namespace OrchardCore.ElsaWorkflows.Contents.Activities;

[Activity("OrchardCore.Content", "Content", "Triggered when a content item is created.")]
public class ContentCreated : ContentEventTriggerBase;