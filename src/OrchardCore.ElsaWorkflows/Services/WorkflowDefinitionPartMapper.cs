using System;
using Elsa.Workflows;
using Elsa.Workflows.Management.Entities;
using Elsa.Workflows.Management.Mappers;
using Elsa.Workflows.Management.Models;
using OrchardCore.ElsaWorkflows.Parts;

namespace OrchardCore.ElsaWorkflows.Services;

public class WorkflowDefinitionPartMapper(IApiSerializer apiSerializer, WorkflowDefinitionMapper mapper)
{
    public WorkflowDefinition Map(WorkflowDefinitionPart part)
    {
        var json = part.SerializedData;
        var definitionModel = apiSerializer.Deserialize<WorkflowDefinitionModel>(json);
        var workflowDefinition = mapper.MapToWorkflowDefinition(definitionModel);
        return workflowDefinition;
    }

    public void Map(WorkflowDefinitionModel source, WorkflowDefinitionPart target)
    {
        target.DefinitionId = source.DefinitionId;
        target.DefinitionVersionId = source.Id;
        target.IsLatest = source.IsLatest;
        target.IsPublished = source.IsPublished;
        target.Version = source.Version;
        target.ToolVersion = source.ToolVersion;
        target.Name = source.Name;
        target.Description = source.Description;
        target.IsReadonly = source.IsReadonly;
        target.IsSystem = source.IsSystem;
        target.UsableAsActivity = source.Options?.UsableAsActivity == true;
        target.SerializedData = apiSerializer.Serialize(source);
    }

    public WorkflowDefinitionSummary MapSummary(WorkflowDefinitionPart part)
    {
        return new()
        {
            CreatedAt = (DateTimeOffset)part.ContentItem.CreatedUtc!,
            Version = part.Version,
            Description = part.Description,
            Name = part.Name,
            Id = part.DefinitionVersionId,
            DefinitionId = part.DefinitionId,
            IsLatest = part.IsLatest,
            IsPublished = part.IsPublished,
            IsReadonly = part.IsReadonly,
            MaterializerName = part.MaterializerName,
            ProviderName = part.ProviderName,
            ToolVersion = part.ToolVersion
        };
    }
}