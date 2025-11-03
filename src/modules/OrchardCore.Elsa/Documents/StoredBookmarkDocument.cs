namespace OrchardCore.Elsa.Documents;

public class StoredBookmarkDocument
{
    /// <summary>
    /// Gets or sets the unique identifier of the bookmark.
    /// </summary>
    public string BookmarkId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ID of the tenant that owns this bookmark.
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// Gets or sets the name of the bookmark.
    /// </summary>
    public string? Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the hash of the bookmark.
    /// </summary>
    public string Hash { get; set; } = null!;

    /// <summary>
    /// Gets or sets the workflow instance ID.
    /// </summary>
    public string WorkflowInstanceId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the activity instance ID.
    /// </summary>
    public string? ActivityInstanceId { get; set; }

    /// <summary>
    /// Gets or sets the correlation ID.
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the serialized payload.
    /// </summary>
    public string? SerializedPayload { get; set; }

    /// <summary>
    /// Gets or sets the serialized metadata.
    /// </summary>
    public string? SerializedMetadata { get; set; }
}
