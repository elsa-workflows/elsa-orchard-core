using Elsa.Workflows.Runtime.Entities;
using YesSql.Indexes;

namespace OrchardCore.ElsaWorkflows.Indexes;

public class StoredTriggerIndexProvider : IndexProvider<StoredTrigger>
{
    public StoredTriggerIndexProvider()
    {
        CollectionName = ElsaCollections.StoredTriggers;
    }
    
    public override void Describe(DescribeContext<StoredTrigger> context)
    {
        context.For<StoredTriggerIndex>().Map(trigger => new()
        {
            TriggerId = trigger.Id,
            WorkflowDefinitionId = trigger.WorkflowDefinitionId,
            WorkflowDefinitionVersionId = trigger.WorkflowDefinitionVersionId,
            Name = trigger.Name,
            ActivityId = trigger.ActivityId,
            Hash = trigger.Hash,
        });
    }
}