using System.Threading;
using System.Threading.Tasks;
using Elsa.Mediator.Contracts;
using Elsa.Mediator.Models;
using Elsa.Workflows;
using Elsa.Workflows.Management.Commands;
using Elsa.Workflows.Management.Mappers;
using OrchardCore.ContentManagement;
using OrchardCore.ElsaWorkflows.Parts;

namespace OrchardCore.ElsaWorkflows.Handlers.Requests;

public class SaveWorkflowDefinitionHandler(IContentManager contentManager, WorkflowDefinitionMapper mapper, IApiSerializer apiSerializer) : ICommandHandler<SaveWorkflowDefinitionCommand>
{
    public async Task<Unit> HandleAsync(SaveWorkflowDefinitionCommand command, CancellationToken cancellationToken)
    {
        var contentItem = await contentManager.GetAsync(command.WorkflowDefinition.DefinitionId, VersionOptions.Draft);
        
        if (contentItem == null)
        {
            contentItem = await contentManager.NewAsync("WorkflowDefinition");
            await contentManager.CreateAsync(contentItem, VersionOptions.Draft);
        }
        
        var workflowDefinitionModel = await mapper.MapAsync(command.WorkflowDefinition, cancellationToken);
        contentItem.Alter<WorkflowDefinitionPart>(part => { part.SerializedData = apiSerializer.Serialize(workflowDefinitionModel); });
        
        if(command.WorkflowDefinition.IsPublished)
            await contentManager.PublishAsync(contentItem);
        else
            await contentManager.SaveDraftAsync(contentItem);
        
        return Unit.Instance;
    }
}