namespace OrchardCore.Elsa.Documents;

public class WorkflowExecutionLogRecordDocument
{
    /// <summary>
    /// Gets or sets the unique identifier of the log record.
    /// </summary>
    public string RecordId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ID of the tenant that owns this log record.
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// Gets or sets the workflow instance ID.
    /// </summary>
    public string WorkflowInstanceId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the workflow definition ID.
    /// </summary>
    public string? WorkflowDefinitionId { get; set; }

    /// <summary>
    /// Gets or sets the workflow definition version ID.
    /// </summary>
    public string? WorkflowDefinitionVersionId { get; set; }

    /// <summary>
    /// Gets or sets the workflow version.
    /// </summary>
    public int WorkflowVersion { get; set; }

    /// <summary>
    /// Gets or sets the activity ID.
    /// </summary>
    public string? ActivityId { get; set; }

    /// <summary>
    /// Gets or sets the activity node ID.
    /// </summary>
    public string? ActivityNodeId { get; set; }

    /// <summary>
    /// Gets or sets the activity type.
    /// </summary>
    public string? ActivityType { get; set; }

    /// <summary>
    /// Gets or sets the activity type version.
    /// </summary>
    public int ActivityTypeVersion { get; set; }

    /// <summary>
    /// Gets or sets the activity instance ID.
    /// </summary>
    public string? ActivityInstanceId { get; set; }

    /// <summary>
    /// Gets or sets the parent activity instance ID.
    /// </summary>
    public string? ParentActivityInstanceId { get; set; }

    /// <summary>
    /// Gets or sets the event name.
    /// </summary>
    public string? EventName { get; set; }

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the source.
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the sequence number.
    /// </summary>
    public long Sequence { get; set; }

    /// <summary>
    /// Gets or sets the serialized payload.
    /// </summary>
    public string? SerializedPayload { get; set; }
}
