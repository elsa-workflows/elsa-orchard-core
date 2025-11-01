using Elsa.Workflows;

namespace OrchardCore.Elsa.Documents;

public class WorkflowInstanceDocument
{
    /// <summary>
    /// Gets or sets the unique identifier of the workflow instance.
    /// </summary>
    public string InstanceId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ID of the tenant that owns this workflow instance.
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the workflow definition.
    /// </summary>
    public string DefinitionId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the version ID of the workflow definition.
    /// </summary>
    public string DefinitionVersionId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the version number of this workflow instance.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Gets or sets the correlation ID.
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the name of the workflow instance.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the parent workflow instance ID.
    /// </summary>
    public string? ParentWorkflowInstanceId { get; set; }

    /// <summary>
    /// Gets or sets the workflow status.
    /// </summary>
    public WorkflowStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the workflow sub-status.
    /// </summary>
    public WorkflowSubStatus SubStatus { get; set; }

    /// <summary>
    /// Gets or sets the incident count.
    /// </summary>
    public int IncidentCount { get; set; }

    /// <summary>
    /// Gets or sets whether this is a system workflow.
    /// </summary>
    public bool IsSystem { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the finish timestamp.
    /// </summary>
    public DateTimeOffset? FinishedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the serialized workflow state.
    /// </summary>
    public string? SerializedWorkflowState { get; set; }
}
