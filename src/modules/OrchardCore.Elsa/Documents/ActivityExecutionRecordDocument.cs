using Elsa.Workflows;
using Elsa.Workflows.State;

namespace OrchardCore.Elsa.Documents;

public class ActivityExecutionRecordDocument
{
    /// <summary>
    /// Gets or sets the ID of this entity.
    /// </summary>
    public string RecordId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ID of the tenant that own this entity.
    /// </summary>
    public string? TenantId { get; set; }
    
        /// <summary>
    /// Gets or sets the workflow instance ID.
    /// </summary>
    public string WorkflowInstanceId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the activity ID.
    /// </summary>
    public string ActivityId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the activity node ID.
    /// </summary>
    public string ActivityNodeId { get; set; } = null!;

    /// <summary>
    /// The type of the activity.
    /// </summary>
    public string ActivityType { get; set; } = null!;

    /// <summary>
    /// The version of the activity type.
    /// </summary>
    public int ActivityTypeVersion { get; set; }

    /// <summary>
    /// The name of the activity.
    /// </summary>
    public string? ActivityName { get; set; }

    /// <summary>
    /// The state of the activity at the time this record is created or last updated.
    /// </summary>
    public string? SerializedActivityState { get; set; }

    /// <summary>
    /// Any additional payload associated with the log record.
    /// </summary>
    public string? SerializedPayload { get; set; }

    /// <summary>
    /// Any outputs provided by the activity.
    /// </summary>
    public string? SerializedOutputs { get; set; }

    /// <summary>
    /// Any properties provided by the activity.
    /// </summary>
    public string? SerializedProperties { get; set; }

    /// <summary>
    /// Lightweight metadata associated with the activity execution.
    /// This information will be retained as part of the activity execution summary record.
    /// </summary>
    public string? SerializedMetadata { get; set; }

    /// <summary>
    /// Gets or sets the exception that occurred during the activity execution.
    /// </summary>
    public string? SerializedException { get; set; }

    /// <summary>
    /// Gets or sets the time at which the activity execution began.
    /// </summary>
    public DateTimeOffset StartedAt { get; set; }

    /// <summary>
    /// Gets or sets whether the activity has any bookmarks.
    /// </summary>
    public bool HasBookmarks { get; set; }

    /// <summary>
    /// Gets or sets the status of the activity.
    /// </summary>
    public ActivityStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the aggregated count of faults encountered during the execution of the activity instance and its descendants.
    /// </summary>
    public int AggregateFaultCount { get; set; }

    /// <summary>
    /// Gets or sets the time at which the activity execution completed.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; set; }
}