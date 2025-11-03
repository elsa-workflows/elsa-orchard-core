namespace OrchardCore.Elsa.Documents;

public class StoredTriggerDocument
{
    /// <summary>
    /// Gets or sets the unique identifier of the trigger.
    /// </summary>
    public string TriggerId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ID of the tenant that owns this trigger.
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// Gets or sets the workflow definition ID.
    /// </summary>
    public string WorkflowDefinitionId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the workflow definition version ID.
    /// </summary>
    public string WorkflowDefinitionVersionId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the name of the trigger.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the activity ID.
    /// </summary>
    public string ActivityId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the hash of the trigger.
    /// </summary>
    public string Hash { get; set; } = null!;

    /// <summary>
    /// Gets or sets the serialized payload.
    /// </summary>
    public string? SerializedPayload { get; set; }
}
