using OrchardCore.ContentManagement;
using OrchardCore.Elsa.Agents.Parts;
using YesSql.Indexes;

namespace OrchardCore.Elsa.Agents.Indexes;

public class AgentIndexProvider : IndexProvider<ContentItem>
{
    public override void Describe(DescribeContext<ContentItem> context)
    {
        context.For<AgentIndex>()
            .Map(contentItem =>
            {
                var part = contentItem.As<AgentPart>();
                if (part == null)
                    return null!;

                return new AgentIndex
                {
                    AgentId = part.AgentId,
                    ContentItemId = contentItem.ContentItemId,
                    ContentItemVersionId = contentItem.ContentItemVersionId,
                    Name = part.Name,
                    Published = contentItem.Published,
                    Latest = contentItem.Latest
                };
            });
    }
}
