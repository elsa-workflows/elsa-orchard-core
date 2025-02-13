using System;
using OrchardCore.ContentManagement;

namespace OrchardCore.ElsaWorkflows.Parts;

public class WorkflowDefinitionPart : ContentPart
{
    public string DefinitionId { get; set; } = null!;
    public string DefinitionVersionId { get; set; } = null!;
    public int Version { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ProviderName { get; set; }
    public string MaterializerName { get; set; }
    public bool IsPublished { get; set; }
    public bool IsLatest { get; set; }
    public bool UsableAsActivity { get; set; }
    public bool IsSystem { get; set; }
    public bool IsReadonly { get; set; }
    public Version? ToolVersion { get; set; }
    public string SerializedData { get; set; } = null!;
}