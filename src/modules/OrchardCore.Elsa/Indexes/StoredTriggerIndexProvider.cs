using JetBrains.Annotations;
using OrchardCore.Elsa.Documents;
using YesSql.Indexes;

namespace OrchardCore.Elsa.Indexes;

[UsedImplicitly]
public class StoredTriggerIndexProvider : IndexProvider<StoredTriggerDocument>
{
    public StoredTriggerIndexProvider()
    {
        CollectionName = ElsaCollections.StoredTriggers;
    }

    public override void Describe(DescribeContext<StoredTriggerDocument> context)
    {
        context.For<StoredTriggerIndex>().Map(document => new()
        {
            TriggerId = document.TriggerId,
            WorkflowDefinitionId = document.WorkflowDefinitionId,
            WorkflowDefinitionVersionId = document.WorkflowDefinitionVersionId,
            Name = document.Name,
            ActivityId = document.ActivityId,
            Hash = document.Hash,
        });
    }
}