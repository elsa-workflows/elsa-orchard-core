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

public class FindWorkflowDefinitionHandler(IContentManager contentManager, WorkflowDefinitionMapper mapper, IApiSerializer apiSerializer) : IRequestHandler<FindWorkflowDefinitionRequest, WorkflowDefinition?>
{
    public async Task<WorkflowDefinition?> HandleAsync(FindWorkflowDefinitionRequest request, CancellationToken cancellationToken)
    {
        var versionOptions = request.VersionOptions.IsPublished ? VersionOptions.Published : request.VersionOptions.IsDraft ? VersionOptions.Draft : VersionOptions.Latest;
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