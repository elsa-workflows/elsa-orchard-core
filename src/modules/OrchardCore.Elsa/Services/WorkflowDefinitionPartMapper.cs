using Elsa.Common;
using Elsa.Workflows;
using Elsa.Workflows.Management.Entities;
using Elsa.Workflows.Management.Mappers;
using Elsa.Workflows.Management.Models;
using OrchardCore.Elsa.Parts;

namespace OrchardCore.Elsa.Services;

public class WorkflowDefinitionPartMapper(IApiSerializer apiSerializer, WorkflowDefinitionMapper mapper, ISystemClock systemClock)
{
    public WorkflowDefinition Map(WorkflowDefinitionPart part)
    {
        var definitionModel = MapModel(part);
        definitionModel.Id = part.DefinitionVersionId;
        definitionModel.DefinitionId = part.DefinitionId;
        definitionModel.Version = part.Version;
        definitionModel.ToolVersion = part.ToolVersion;
        definitionModel.Name = part.Name;
        definitionModel.Description = part.Description;
        definitionModel.IsLatest = part.IsLatest;
        definitionModel.IsPublished = part.IsPublished;
        definitionModel.IsReadonly = part.IsReadonly;
        definitionModel.IsSystem = part.IsSystem;
        
        if(part.ContentItem.CreatedUtc != null)
            definitionModel.CreatedAt = (DateTimeOffset)part.ContentItem.CreatedUtc;
        
        return mapper.MapToWorkflowDefinition(definitionModel);
    }
    
    public WorkflowDefinitionModel MapModel(WorkflowDefinitionPart part)
    {
        var model = part.SerializedData != null! ? apiSerializer.Deserialize<WorkflowDefinitionModel>(part.SerializedData) : new();
        
        return new()
        {
            CreatedAt = part.ContentItem.CreatedUtc ?? systemClock.UtcNow,
            Version = part.Version,
            Description = part.Description,
            Name = part.Name,
            Id = part.DefinitionVersionId,
            DefinitionId = part.DefinitionId,
            IsLatest = part.IsLatest,
            IsPublished = part.IsPublished,
            IsReadonly = part.IsReadonly,
            ToolVersion = part.ToolVersion,
            IsSystem = part.IsSystem,
            Inputs = model.Inputs,
            Options = model.Options,
            Outcomes = model.Outcomes,
            Outputs = model.Outputs,
            Root = model.Root,
            Variables = model.Variables,
            CustomProperties = model.CustomProperties,
            TenantId = model.TenantId
        };
    }

    public void Map(WorkflowDefinitionModel source, WorkflowDefinitionPart target)
    {
        target.DefinitionId = source.DefinitionId;
        target.DefinitionVersionId = source.Id;
        target.IsLatest = source.IsLatest;
        target.IsPublished = source.IsPublished;
        target.Version = source.Version;
        target.ToolVersion = source.ToolVersion;
        target.Name = source.Name!;
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