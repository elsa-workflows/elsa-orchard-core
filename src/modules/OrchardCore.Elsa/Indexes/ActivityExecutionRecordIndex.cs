using Elsa.Workflows;
using YesSql.Indexes;

namespace OrchardCore.Elsa.Indexes;

public class ActivityExecutionRecordIndex : MapIndex
{
    public string RecordId { get; set; } = null!;
    public string WorkflowInstanceId { get; set; } = null!;
    public string ActivityId { get; set; } = null!;
    public string ActivityNodeId { get; set; } = null!;
    public string ActivityType { get; set; } = null!;
    public int ActivityTypeVersion { get; set; }
    public string? ActivityName { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public bool HasBookmarks { get; set; }
    public ActivityStatus Status { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public bool Completed { get; set; }
}