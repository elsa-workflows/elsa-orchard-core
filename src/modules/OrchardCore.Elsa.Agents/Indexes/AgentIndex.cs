using YesSql.Indexes;

namespace OrchardCore.Elsa.Agents.Indexes;

public class AgentIndex : MapIndex
{
    public string AgentId { get; set; } = default!;
    public string ContentItemId { get; set; } = default!;
    public string ContentItemVersionId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public bool Published { get; set; }
    public bool Latest { get; set; }
}
