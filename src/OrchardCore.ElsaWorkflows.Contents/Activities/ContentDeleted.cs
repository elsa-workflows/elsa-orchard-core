using Elsa.Workflows.Attributes;

namespace OrchardCore.ElsaWorkflows.Contents.Activities;

[Activity("OrchardCore.Content", "Content", "Triggered when a content item has been deleted.")]
public class ContentDeleted : ContentEventTriggerBase;