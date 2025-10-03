using Elsa.Workflows;
using YesSql.Indexes;

namespace OrchardCore.Elsa.Indexes;

public class WorkflowInstanceIndex : MapIndex
{
    public string InstanceId { get; set; } = null!;
    public string DefinitionId { get; set; } = null!;
    public string DefinitionVersionId { get; set; } = null!;
    public int Version { get; set; }
    public string? CorrelationId { get; set; }
    public string? Name { get; set; }
    public string? ParentInstanceId { get; set; }    
    public WorkflowStatus Status { get; set; }
    public WorkflowSubStatus SubStatus { get; set; }
    public bool HasIncidents { get; set; }
    public int IncidentCount { get; set; }
    public bool IsSystem { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? FinishedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}