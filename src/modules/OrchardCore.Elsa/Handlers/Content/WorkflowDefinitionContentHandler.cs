using Elsa.Mediator.Contracts;
using Elsa.Workflows;
using Elsa.Workflows.Management.Entities;
using Elsa.Workflows.Management.Mappers;
using Elsa.Workflows.Management.Models;
using Elsa.Workflows.Management.Notifications;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.Elsa.Parts;
using OrchardCore.Elsa.Services;

namespace OrchardCore.Elsa.Handlers.Content;

public class WorkflowDefinitionContentHandler(
    IMediator mediator,
    IApiSerializer apiSerializer,
    IServiceProvider serviceProvider
) : ContentHandlerBase
{
    private readonly Lazy<WorkflowDefinitionPartMapper> _workflowDefinitionPartMapper = new(serviceProvider.GetRequiredService<WorkflowDefinitionPartMapper>);
    private readonly Lazy<WorkflowDefinitionMapper> _workflowDefinitionMapper = new(serviceProvider.GetRequiredService<WorkflowDefinitionMapper>());
    private readonly Lazy<WorkflowDefinitionPartSerializer> _workflowDefinitionPartSerializer = new(serviceProvider.GetRequiredService<WorkflowDefinitionPartSerializer>);
    private WorkflowDefinitionPartMapper WorkflowDefinitionPartMapper => _workflowDefinitionPartMapper.Value;
    private WorkflowDefinitionMapper WorkflowDefinitionMapper => _workflowDefinitionMapper.Value;
    private WorkflowDefinitionPartSerializer WorkflowDefinitionPartSerializer => _workflowDefinitionPartSerializer.Value;

    public override Task GetContentItemAspectAsync(ContentItemAspectContext context)
    {
        if (!context.ContentItem.Has<WorkflowDefinitionPart>())
            return Task.CompletedTask;

        return context.ForAsync<ContentItemMetadata>(metadata =>
        {
            metadata.CreateRouteValues = new()
            {
                { "Area", Constants.Area },
                { "Controller", "WorkflowDefinitions" },
                { "Action", "Create" },
                { "Id", context.ContentItem.ContentType },
            };

            metadata.EditorRouteValues = new()
            {
                { "Area", Constants.Area },
                { "Controller", "WorkflowDefinitions" },
                { "Action", "Edit" },
                { "Id", context.ContentItem.ContentItemId },
            };

            metadata.AdminRouteValues = new()
            {
                { "Area", Constants.Area },
                { "Controller", "WorkflowDefinitions" },
                { "Action", "Edit" },
                { "Id", context.ContentItem.ContentItemId },
            };

            return Task.CompletedTask;
        });
    }

    public override Task UpdatingAsync(UpdateContentContext context)
    {
        if (!context.ContentItem.Has<WorkflowDefinitionPart>())
            return Task.CompletedTask;
        
        var workflowDefinitionPart = context.ContentItem.As<WorkflowDefinitionPart>();
        context.ContentItem.DisplayText = workflowDefinitionPart.Name;
        return Task.CompletedTask;
    }

    public override async Task PublishingAsync(PublishContentContext context)
    {
        if (!context.ContentItem.Has<WorkflowDefinitionPart>())
            return;

        var newItem = context.ContentItem;
        var previousItem = context.PreviousItem;
        
        await newItem.AlterAsync<WorkflowDefinitionPart>(async part =>
        {
            part.DefinitionVersionId = newItem.ContentItemVersionId;
            part.IsLatest = true;
            part.IsPublished = true;
            newItem.DisplayText = part.Name;
            var definitionModel = WorkflowDefinitionPartSerializer.UpdateSerializedData(part);
            var definition = WorkflowDefinitionMapper.MapToWorkflowDefinition(definitionModel);
            await mediator.SendAsync(new WorkflowDefinitionPublishing(definition));
        });

        previousItem?.Alter<WorkflowDefinitionPart>(part =>
        {
            part.IsPublished = false;
            part.IsLatest = false;
            WorkflowDefinitionPartSerializer.UpdateSerializedData(part);
        });
    }

    public override async Task PublishedAsync(PublishContentContext context)
    {
        if (!context.ContentItem.Has<WorkflowDefinitionPart>())
            return;

        var previousItem = context.PreviousItem;

        if (previousItem != null)
        {
            var previousDefinitionPart = previousItem.As<WorkflowDefinitionPart>();
            var previousDefinition = WorkflowDefinitionPartMapper.Map(previousDefinitionPart);
            await mediator.SendAsync(new WorkflowDefinitionVersionRetracted(previousDefinition));
        }
        
        var workflowDefinitionPart = context.ContentItem.As<WorkflowDefinitionPart>();
        var workflowDefinition = WorkflowDefinitionPartMapper.Map(workflowDefinitionPart);
        var affectedWorkflows = new AffectedWorkflows(new List<WorkflowDefinition>());
        await mediator.SendAsync(new WorkflowDefinitionPublished(workflowDefinition, affectedWorkflows));
    }

    public override async Task UnpublishingAsync(PublishContentContext context)
    {
        if (!context.ContentItem.Has<WorkflowDefinitionPart>())
            return;

        context.ContentItem.Alter<WorkflowDefinitionPart>(part =>
        {
            part.IsPublished = false;
            part.IsLatest = false;
            WorkflowDefinitionPartSerializer.UpdateSerializedData(part);
        });

        var workflowDefinitionPart = context.ContentItem.As<WorkflowDefinitionPart>();
        var workflowDefinition = WorkflowDefinitionPartMapper.Map(workflowDefinitionPart);
        await mediator.SendAsync(new WorkflowDefinitionRetracted(workflowDefinition));
    }

    public override async Task RemovingAsync(RemoveContentContext context)
    {
        var workflowDefinitionPart = context.ContentItem.As<WorkflowDefinitionPart>();
        await mediator.SendAsync(new WorkflowDefinitionDeleting(workflowDefinitionPart.DefinitionId));
    }
}