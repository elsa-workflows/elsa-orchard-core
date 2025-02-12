using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Mediator.Contracts;
using Elsa.Workflows;
using Elsa.Workflows.Management.Entities;
using Elsa.Workflows.Management.Mappers;
using Elsa.Workflows.Management.Models;
using Elsa.Workflows.Management.Requests;
using Elsa.Workflows.Models;
using OrchardCore.ContentManagement;
using OrchardCore.ElsaWorkflows.Parts;

namespace OrchardCore.ElsaWorkflows.Handlers.Requests;

public class FindWorkflowDefinitionHandler(IContentManager contentManager, WorkflowDefinitionMapper mapper, IApiSerializer apiSerializer) : IRequestHandler<FindWorkflowDefinitionRequest, WorkflowDefinition?>
{
    public async Task<WorkflowDefinition?> HandleAsync(FindWorkflowDefinitionRequest request, CancellationToken cancellationToken)
    {
        var handle = request.Handle;
        var contentItem =  await GetContentItemAsync(handle, cancellationToken);
        
        if (contentItem == null)
            return null;
        
        var workflowDefinitionPart = contentItem.As<WorkflowDefinitionPart>();
        var json = workflowDefinitionPart.SerializedData;
        var definitionModel = apiSerializer.Deserialize<WorkflowDefinitionModel>(json);
        var workflowDefinition = mapper.MapToWorkflowDefinition(definitionModel);
        return workflowDefinition;
    }

    private async Task<ContentItem?> GetContentItemAsync(WorkflowDefinitionHandle handle, CancellationToken cancellationToken)
    {
        var definitionVersionId = handle.DefinitionVersionId;
        
        if(definitionVersionId != null)
            return await contentManager.GetVersionAsync(definitionVersionId);
        
        var definitionId = handle.DefinitionId!;
        var wfVersionOptions = handle.VersionOptions!.Value;
        var ocVersionOptions = wfVersionOptions.IsPublished ? VersionOptions.Published : wfVersionOptions.IsDraft ? VersionOptions.Draft : VersionOptions.Latest;
        return await contentManager.GetAsync(definitionId, ocVersionOptions);
    }
}