using OrchardCore.ContentManagement;
using YesSql.Indexes;

namespace OrchardCore.ElsaWorkflows.Indexes;

public class WorkflowDefinitionIndexProvider : IndexProvider<ContentItem>
{
    public override void Describe(DescribeContext<ContentItem> context)
    {
        context.For<WorkflowDefinitionIndex>()
            .Map(workflowDefinition => new()
            {
                WorkflowDefinitionId = workflowDefinition.ContentItemId,
                Name = workflowDefinition.DisplayText
            });
    }
}
