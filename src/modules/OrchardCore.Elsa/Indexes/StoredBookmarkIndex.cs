using YesSql.Indexes;

namespace OrchardCore.Elsa.Indexes;

public class StoredBookmarkIndex : MapIndex
{
    public string BookmarkId { get; set; } = null!;
    public string? Name { get; set; }
    public string WorkflowInstanceId { get; set; } = null!;
    public string Hash { get; set; } = null!;
    public string? CorrelationId { get; set; }
    public string? ActivityInstanceId { get; set; }
}