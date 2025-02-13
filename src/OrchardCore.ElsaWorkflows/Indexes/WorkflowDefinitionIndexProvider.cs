using OrchardCore.ContentManagement;
using OrchardCore.ElsaWorkflows.Parts;
using YesSql.Indexes;

namespace OrchardCore.ElsaWorkflows.Indexes;

public class WorkflowDefinitionIndexProvider : IndexProvider<ContentItem>
{
    public override void Describe(DescribeContext<ContentItem> context)
    {
        context.For<WorkflowDefinitionIndex>()
            .Map(contentItem =>
            {
                var workflowDefinitionPart = contentItem.As<WorkflowDefinitionPart>();
                return new()
                {
                    DefinitionId = contentItem.ContentItemId,
                    DefinitionVersionId = contentItem.ContentItemVersionId,
                    IsLatest = workflowDefinitionPart.IsLatest,
                    IsPublished = workflowDefinitionPart.IsPublished,
                    IsSystem = workflowDefinitionPart.IsSystem,
                    UsableAsActivity = workflowDefinitionPart.UsableAsActivity,
                    MaterializerName = workflowDefinitionPart.MaterializerName,
                    IsReadonly = workflowDefinitionPart.IsReadonly,
                    Version = workflowDefinitionPart.Version,
                    Name = contentItem.DisplayText,
                    Description = workflowDefinitionPart.Description,
                };
            });
    }
}
