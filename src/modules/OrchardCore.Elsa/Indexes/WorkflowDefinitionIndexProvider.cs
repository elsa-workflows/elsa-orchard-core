using OrchardCore.ContentManagement;
using OrchardCore.Elsa.Parts;
using YesSql.Indexes;

namespace OrchardCore.Elsa.Indexes;

public class WorkflowDefinitionIndexProvider : IndexProvider<ContentItem>
{
    public override void Describe(DescribeContext<ContentItem> context)
    {
        context.For<WorkflowDefinitionIndex>()
            .Map(contentItem =>
            {
                var workflowDefinitionPart = contentItem.As<WorkflowDefinitionPart>();
                
                if(workflowDefinitionPart == null)
                    return null!;
                
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
                    Name = workflowDefinitionPart.Name,
                    Description = workflowDefinitionPart.Description,
                };
            });
    }
}