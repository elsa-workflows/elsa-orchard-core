using YesSql.Indexes;

namespace OrchardCore.Elsa.Agents.Indexes;

public class ApiKeyDefinitionIndex : MapIndex
{
    public string ApiKeyId { get; set; } = default!;
    public string Name { get; set; } = default!;
}
