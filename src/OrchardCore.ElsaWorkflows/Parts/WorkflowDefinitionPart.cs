using OrchardCore.ContentManagement;

namespace OrchardCore.ElsaWorkflows.Parts;

public class WorkflowDefinitionPart : ContentPart
{
    public string SerializedData { get; set; } = null!;
}