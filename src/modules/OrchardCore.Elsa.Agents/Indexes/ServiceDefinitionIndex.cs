using YesSql.Indexes;

namespace OrchardCore.Elsa.Agents.Indexes;

public class ServiceDefinitionIndex : MapIndex
{
    public string ServiceId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Type { get; set; } = default!;
}
