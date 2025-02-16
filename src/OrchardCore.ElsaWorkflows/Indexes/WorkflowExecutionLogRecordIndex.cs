using System;
using YesSql.Indexes;

namespace OrchardCore.ElsaWorkflows.Indexes;

public class WorkflowExecutionLogRecordIndex : MapIndex
{
    public string RecordId { get; set; } = null!;
    public string WorkflowInstanceId { get; set; } = null!;
    public string? ParentActivityInstanceId { get; set; }
    public string ActivityId { get; set; } = null!;
    public string ActivityNodeId { get; set; } = null!;
    public string? EventName { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public long Sequence { get; set; }
}