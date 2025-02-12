using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Mediator.Contracts;
using Elsa.Workflows;
using Elsa.Workflows.Management.Entities;
using Elsa.Workflows.Management.Mappers;
using Elsa.Workflows.Management.Models;
using Elsa.Workflows.Management.Requests;
using OrchardCore.ContentManagement;
using OrchardCore.ElsaWorkflows.Parts;

namespace OrchardCore.ElsaWorkflows.Handlers.Requests;

public class FindLastVersionOfWorkflowDefinitionHandler(IContentManager contentManager, WorkflowDefinitionMapper mapper, IApiSerializer apiSerializer) : IRequestHandler<FindLastVersionOfWorkflowDefinitionRequest, WorkflowDefinition?>
{
    public async Task<WorkflowDefinition?> HandleAsync(FindLastVersionOfWorkflowDefinitionRequest request, CancellationToken cancellationToken)
    {
        var versionOptions = VersionOptions.Latest;
        var contentItem = await contentManager.GetAsync(request.DefinitionId, versionOptions);
        
        if (contentItem == null)
            return null;
        
        var workflowDefinitionPart = contentItem.As<WorkflowDefinitionPart>();
        var json = workflowDefinitionPart.SerializedData;
        var definitionModel = apiSerializer.Deserialize<WorkflowDefinitionModel>(json);
        var workflowDefinition = mapper.MapToWorkflowDefinition(definitionModel);
        return workflowDefinition;
    }
}

public class FindLatestOrPublishedWorkflowDefinitionsHandler(IContentManager contentManager, WorkflowDefinitionMapper mapper, IApiSerializer apiSerializer) : IRequestHandler<FindLatestOrPublishedWorkflowDefinitionsRequest, ICollection<WorkflowDefinition>>
{
    public async Task<ICollection<WorkflowDefinition>> HandleAsync(FindLatestOrPublishedWorkflowDefinitionsRequest request, CancellationToken cancellationToken)
    {
        var contentItem = await contentManager.GetAsync(request.DefinitionId, VersionOptions.Published) 
                          ?? await contentManager.GetAsync(request.DefinitionId, VersionOptions.Latest);

        var workflowDefinitionPart = contentItem.As<WorkflowDefinitionPart>();
        var json = workflowDefinitionPart.SerializedData;
        var definitionModel = apiSerializer.Deserialize<WorkflowDefinitionModel>(json);
        var workflowDefinition = mapper.MapToWorkflowDefinition(definitionModel);
        return [workflowDefinition];
    }
}