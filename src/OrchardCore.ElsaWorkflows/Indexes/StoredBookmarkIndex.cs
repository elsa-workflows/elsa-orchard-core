using YesSql.Indexes;

namespace OrchardCore.ElsaWorkflows.Indexes;

public class StoredBookmarkIndex : MapIndex
{
    public string BookmarkId { get; set; } = null!;
    public string WorkflowInstanceId { get; set; } = null!;
    public string Hash { get; set; } = null!;
    public string? CorrelationId { get; set; }
    public string ActivityTypeName { get; set; } = null!;
    public string? ActivityInstanceId { get; set; }
}